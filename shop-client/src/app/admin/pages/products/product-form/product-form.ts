import {
  Component,
  inject,
  OnDestroy,
  signal,
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  catchError,
  concatMap,
  finalize,
  from,
  map,
  Observable,
  of,
  switchMap,
  tap,
  toArray,
} from 'rxjs';

import { MatIconModule } from '@angular/material/icon';

import {
  AdminProductImageResponse,
  AdminProductService,
  CreateAdminProductRequest,
  UpdateAdminProductRequest,
} from '../../../services/admin-product.service';

import {
  AdminProductOptionDefinitionResponse,
  AdminProductOptionDefinitionService,
  ProductOptionInputType,
} from '../../../services/admin-product-option-definition.service';

import { CategoryService } from '../../../../core/services/category/category.service';

import {
  AdminFileService,
} from '../../../services/admin-file.service';

import { environment } from '../../../../../environments/environment';


interface PendingProductImage {
  file: File;
  previewUrl: string;
}
@Component({
  selector: 'app-admin-product-form',

  imports: [ReactiveFormsModule, RouterLink, MatIconModule],

  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class AdminProductForm implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fileService =
    inject(AdminFileService);

  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(AdminProductService);

  private readonly optionDefinitionService = inject(AdminProductOptionDefinitionService);

  readonly categories = toSignal(this.categoryService.getAll().pipe(catchError(() => of([]))), {
    initialValue: [],
  });

  readonly optionDefinitions = toSignal(
    this.optionDefinitionService.getAll().pipe(
      map((definitions) => definitions.filter((definition) => definition.isActive)),
      catchError(() => of([])),
    ),
    {
      initialValue: [],
    },
  );

  readonly productId = Number(this.route.snapshot.paramMap.get('id'));

  readonly isEditMode = this.productId > 0;

  readonly existingImages =
    signal<AdminProductImageResponse[]>([]);

  readonly pendingImages =
    signal<PendingProductImage[]>([]);

  readonly imageError =
    signal<string | null>(null);

  readonly isUploadingImages =
    signal(false);

  readonly selectedOptionDefinitionId = this.fb.nonNullable.control(0);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],

    slug: ['', [Validators.required, Validators.maxLength(200)]],

    categoryId: [0, [Validators.required, Validators.min(1)]],

    description: [''],

    price: [0, [Validators.required, Validators.min(0)]],

    displayOrder: [0, [Validators.required, Validators.min(0)]],

    options: this.fb.array([]),
  });

  get options(): FormArray {
    return this.form.controls.options;
  }

  constructor() {
    if (this.isEditMode) {
      this.loadProduct();
    }
  }


  private createOptionGroup(
    productOptionDefinitionId: number,
    id: number | null = null,
    isRequired = false,
    displayOrder = this.options.length + 1,
  ): FormGroup {
    return this.fb.group({
      id: [id],

      productOptionDefinitionId: [
        productOptionDefinitionId,
        [
          Validators.required,
          Validators.min(1),
        ],
      ],

      isRequired: [isRequired],

      displayOrder: [
        displayOrder,
        [
          Validators.required,
          Validators.min(0),
        ],
      ],

      values: this.fb.array([]),
    });
  }

  private createOptionValueGroup(
    displayOrder: number,
    id: number | null = null,
    label = '',
    value = '',
    priceAdjustment = 0,
    colorCode: string | null = null,
  ): FormGroup {
    return this.fb.group({
      id: [id],

      label: [
        label,
        [
          Validators.required,
          Validators.maxLength(200),
        ],
      ],

      value: [
        value,
        [
          Validators.required,
          Validators.maxLength(200),
        ],
      ],

      priceAdjustment: [
        priceAdjustment,
        [
          Validators.required,
        ],
      ],

      colorCode: [
        colorCode ?? '',
      ],

      displayOrder: [
        displayOrder,
        [
          Validators.required,
          Validators.min(0),
        ],
      ],
    });
  }

  addOption(productOptionDefinitionId: number): void {
    if (!productOptionDefinitionId || productOptionDefinitionId <= 0) {
      return;
    }

    const alreadyExists = this.options.controls.some(
      (control) =>
        Number(control.get('productOptionDefinitionId')?.value) === productOptionDefinitionId,
    );

    if (alreadyExists) {
      return;
    }

    this.options.push(this.createOptionGroup(productOptionDefinitionId));
  }

  addSelectedOption(): void {
    const definitionId = this.selectedOptionDefinitionId.value;

    if (definitionId <= 0) {
      return;
    }

    this.addOption(definitionId);

    this.selectedOptionDefinitionId.reset(0);
  }

  removeOption(optionIndex: number): void {
    this.options.removeAt(optionIndex);
  }

  getOptionValues(optionIndex: number): FormArray {
    return this.options.at(optionIndex).get('values') as FormArray;
  }

  addOptionValue(optionIndex: number): void {
    const values = this.getOptionValues(optionIndex);

    values.push(this.createOptionValueGroup(values.length + 1));
  }

  removeOptionValue(optionIndex: number, valueIndex: number): void {
    this.getOptionValues(optionIndex).removeAt(valueIndex);
  }

  getOptionDefinition(optionIndex: number): AdminProductOptionDefinitionResponse | undefined {
    const definitionId = Number(
      this.options.at(optionIndex).get('productOptionDefinitionId')?.value,
    );

    return this.optionDefinitions().find((definition) => definition.id === definitionId);
  }

  isColorOption(optionIndex: number): boolean {
    return this.getOptionDefinition(optionIndex)?.inputType === ProductOptionInputType.Color;
  }

  private loadProduct(): void {
    this.productService
      .getById(this.productId)
      .subscribe({
        next: (product) => {
          // =========================
          // Base Information
          // =========================

          this.form.patchValue({
            name: product.name,
            slug: product.slug,
            categoryId: product.categoryId,
            description:
              product.description ?? '',
            price: product.price,
            displayOrder:
              product.displayOrder,
          });

          this.existingImages.set(
            [...product.images].sort(
              (firstImage, secondImage) =>
                firstImage.displayOrder -
                secondImage.displayOrder,
            ),
          );

          // =========================
          // Clear Current Options
          // =========================

          this.options.clear();

          // =========================
          // Load Product Options
          // =========================

          const activeOptions =
            product.options
              .filter(
                (option) => option.isActive,
              )
              .sort(
                (a, b) =>
                  a.displayOrder -
                  b.displayOrder,
              );

          for (const option of activeOptions) {
            const optionGroup =
              this.createOptionGroup(
                option.productOptionDefinitionId,
                option.id,
                option.isRequired,
                option.displayOrder,
              );

            const values =
              optionGroup.get(
                'values',
              ) as FormArray;

            const activeValues =
              option.values
                .filter(
                  (value) => value.isActive,
                )
                .sort(
                  (a, b) =>
                    a.displayOrder -
                    b.displayOrder,
                );

            for (const value of activeValues) {
              values.push(
                this.createOptionValueGroup(
                  value.displayOrder,
                  value.id,
                  value.label,
                  value.value,
                  value.priceAdjustment,
                  value.colorCode,
                ),
              );
            }

            this.options.push(
              optionGroup,
            );
          }
        },

        error: (error) => {
          console.error(
            'Load product failed:',
            error,
          );
        },
      });
  }
  onImageSelected(event: Event): void {
    const input =
      event.target as HTMLInputElement;

    const files =
      Array.from(input.files ?? []);

    input.value = '';

    if (files.length === 0) {
      return;
    }

    this.imageError.set(null);

    const allowedTypes = [
      'image/jpeg',
      'image/png',
      'image/webp',
    ];

    const maximumFileSize =
      5 * 1024 * 1024;

    const acceptedImages:
      PendingProductImage[] = [];

    for (const file of files) {
      if (!allowedTypes.includes(file.type)) {
        this.imageError.set(
          'فقط تصاویر JPG، PNG و WebP مجاز هستند.',
        );

        continue;
      }

      if (file.size > maximumFileSize) {
        this.imageError.set(
          'حجم هر تصویر نباید بیشتر از ۵ مگابایت باشد.',
        );

        continue;
      }

      acceptedImages.push({
        file,
        previewUrl:
          URL.createObjectURL(file),
      });
    }

    this.pendingImages.update(
      currentImages => [
        ...currentImages,
        ...acceptedImages,
      ],
    );
  }

  removePendingImage(index: number): void {
    const image =
      this.pendingImages()[index];

    if (!image) {
      return;
    }

    URL.revokeObjectURL(
      image.previewUrl,
    );

    this.pendingImages.update(
      currentImages =>
        currentImages.filter(
          (_, imageIndex) =>
            imageIndex !== index,
        ),
    );
  }

  ngOnDestroy(): void {
    for (const image of this.pendingImages()) {
      URL.revokeObjectURL(
        image.previewUrl,
      );
    }
  }


  private uploadPendingImages(
    productId: number,
  ): Observable<void[]> {
    const images =
      [...this.pendingImages()];

    if (images.length === 0) {
      return of([]);
    }

    const startingDisplayOrder =
      this.existingImages().length;

    return from(images).pipe(
      concatMap(
        (image, index) =>
          this.fileService
            .uploadImage(image.file)
            .pipe(
              switchMap(
                uploadedFile =>
                  this.productService.addImage(
                    productId,
                    {
                      url: uploadedFile.url,

                      displayOrder:
                        startingDisplayOrder +
                        index,

                      isPrimary: false,
                    },
                  ),
              ),

              tap(() => {
                URL.revokeObjectURL(
                  image.previewUrl,
                );

                this.pendingImages.update(
                  currentImages =>
                    currentImages.filter(
                      currentImage =>
                        currentImage !== image,
                    ),
                );
              }),
            ),
      ),

      toArray(),
    );
  }

  private handleSaveError(
    message: string,
    error: unknown,
  ): void {
    console.error(
      message,
      error,
    );

    this.imageError.set(
      'ذخیره محصول یا آپلود تصاویر با خطا مواجه شد. دوباره تلاش کنید.',
    );
  }


  getImageSource(imageUrl: string): string {
  if (
    imageUrl.startsWith('http://') ||
    imageUrl.startsWith('https://')
  ) {
    return imageUrl;
  }

  const apiOrigin =
    environment.apiUrl.replace(
      /\/api\/?$/i,
      '',
    );

  const normalizedImageUrl =
    imageUrl.startsWith('/')
      ? imageUrl
      : `/${imageUrl}`;

  return `${apiOrigin}${normalizedImageUrl}`;
}

setPrimaryImage(imageId: number): void {
  if (!this.isEditMode ||
      this.isUploadingImages()) {
    return;
  }

  this.isUploadingImages.set(true);
  this.imageError.set(null);

  this.productService
    .setPrimaryImage(
      this.productId,
      imageId,
    )
    .pipe(
      finalize(() => {
        this.isUploadingImages.set(false);
      }),
    )
    .subscribe({
      next: () => {
        this.existingImages.update(
          images =>
            images.map(image => ({
              ...image,
              isPrimary:
                image.id === imageId,
            })),
        );
      },

      error: error => {
        this.handleSaveError(
          'Set primary image failed:',
          error,
        );
      },
    });
}

deleteExistingImage(imageId: number): void {
  if (!this.isEditMode ||
      this.isUploadingImages()) {
    return;
  }

  const shouldDelete =
    window.confirm(
      'آیا از حذف این تصویر مطمئن هستید؟',
    );

  if (!shouldDelete) {
    return;
  }

  this.isUploadingImages.set(true);
  this.imageError.set(null);

  this.productService
    .deleteImage(
      this.productId,
      imageId,
    )
    .pipe(
      finalize(() => {
        this.isUploadingImages.set(false);
      }),
    )
    .subscribe({
      next: () => {
        this.existingImages.update(
          images =>
            images.filter(
              image =>
                image.id !== imageId,
            ),
        );
      },

      error: error => {
        this.handleSaveError(
          'Delete product image failed:',
          error,
        );
      },
    });
}


  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    const baseRequest = {
      categoryId: Number(value.categoryId),

      name: value.name.trim(),

      slug: value.slug.trim(),

      description: value.description?.trim() || null,

      price: Number(value.price),

      displayOrder: Number(value.displayOrder),
    };

    // =========================
    // Edit
    // =========================

    if (this.isEditMode) {
      const updateRequest: UpdateAdminProductRequest = {
        ...baseRequest,

        options: this.options.controls.map((optionControl) => {
          const optionValue = optionControl.getRawValue();

          return {
            id: optionValue.id ?? null,

            productOptionDefinitionId: Number(
              optionValue.productOptionDefinitionId,
            ),

            isRequired: Boolean(
              optionValue.isRequired,
            ),

            displayOrder: Number(
              optionValue.displayOrder,
            ),

            values: (optionValue.values ?? []).map(
              (valueItem: any) => ({
                id: valueItem.id ?? null,

                label: valueItem.label.trim(),

                value: valueItem.value.trim(),

                priceAdjustment: Number(
                  valueItem.priceAdjustment,
                ),

                colorCode:
                  valueItem.colorCode?.trim() || null,

                displayOrder: Number(
                  valueItem.displayOrder,
                ),
              }),
            ),
          };
        }),
      };

      this.isUploadingImages.set(true);
      this.imageError.set(null);

      this.productService
        .update(
          this.productId,
          updateRequest,
        )
        .pipe(
          switchMap(() =>
            this.uploadPendingImages(
              this.productId,
            ),
          ),

          finalize(() => {
            this.isUploadingImages.set(false);
          }),
        )
        .subscribe({
          next: () => {
            this.router.navigate([
              '/admin/products',
            ]);
          },

          error: error => {
            this.handleSaveError(
              'Update product failed:',
              error,
            );
          },
        });

      return;
    }

    // =========================
    // Create
    // =========================

    const createRequest: CreateAdminProductRequest = {
      ...baseRequest,

      options: this.options.controls.map(
        (optionControl) => {
          const optionValue =
            optionControl.getRawValue();

          return {
            productOptionDefinitionId: Number(
              optionValue.productOptionDefinitionId,
            ),

            isRequired: Boolean(
              optionValue.isRequired,
            ),

            displayOrder: Number(
              optionValue.displayOrder,
            ),

            values: (optionValue.values ?? []).map(
              (valueItem: any) => ({
                label: valueItem.label.trim(),

                value: valueItem.value.trim(),

                priceAdjustment: Number(
                  valueItem.priceAdjustment,
                ),

                colorCode:
                  valueItem.colorCode?.trim() || null,

                displayOrder: Number(
                  valueItem.displayOrder,
                ),
              }),
            ),
          };
        },
      ),
    };

this.isUploadingImages.set(true);
this.imageError.set(null);

let createdProductId:
  number | null = null;

this.productService
  .create(createRequest)
  .pipe(
    tap(response => {
      createdProductId =
        response.id;
    }),

    switchMap(response =>
      this.uploadPendingImages(
        response.id,
      ),
    ),

    finalize(() => {
      this.isUploadingImages.set(false);
    }),
  )
  .subscribe({
    next: () => {
      this.router.navigate([
        '/admin/products',
      ]);
    },

    error: error => {
      this.handleSaveError(
        'Create product or image upload failed:',
        error,
      );

      if (createdProductId !== null) {
        this.router.navigate([
          '/admin/products',
          createdProductId,
        ]);
      }
    },
  });
  }

  formatPrice(value: number | null): string {
    if (value === null || value === undefined) {
      return '';
    }

    return value.toLocaleString('en-US');
  }

  onPriceInput(event: Event): void {
    const input = event.target as HTMLInputElement;

    const rawValue = input.value.replace(/,/g, '');

    if (!/^\d*$/.test(rawValue)) {
      input.value = this.formatPrice(this.form.controls.price.value);

      return;
    }

    const numericValue = rawValue === '' ? 0 : Number(rawValue);

    this.form.controls.price.setValue(numericValue);

    input.value = rawValue ? numericValue.toLocaleString('en-US') : '';
  }
}
