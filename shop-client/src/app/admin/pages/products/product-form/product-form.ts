import { Component, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of } from 'rxjs';

import { MatIconModule } from '@angular/material/icon';

import {
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

@Component({
  selector: 'app-admin-product-form',

  imports: [ReactiveFormsModule, RouterLink, MatIconModule],

  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class AdminProductForm {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

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

    this.productService
      .update(
        this.productId,
        updateRequest,
      )
      .subscribe({
        next: () => {
          this.router.navigate([
            '/admin/products',
          ]);
        },

        error: (error) => {
          console.error(
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

  this.productService
    .create(createRequest)
    .subscribe({
      next: (response) => {
        console.log(
          'Product created:',
          response,
        );

        this.router.navigate([
          '/admin/products',
        ]);
      },

      error: (error) => {
        console.error(
          'Create product failed:',
          error,
        );
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
