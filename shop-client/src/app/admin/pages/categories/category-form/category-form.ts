import { Component, inject, OnDestroy, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { finalize, map, Observable, of, switchMap } from 'rxjs';
import {
  AdminCategoryService,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '../../../services/admin-category.service';
import { AdminFileService } from '../../../services/admin-file.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-admin-category-form',
  imports: [ReactiveFormsModule, RouterLink, MatIconModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss',
})
export class AdminCategoryForm implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly categoryService = inject(AdminCategoryService);
  private readonly fileService = inject(AdminFileService);

  readonly categoryId = Number(this.route.snapshot.paramMap.get('id'));
  readonly isEditMode = this.categoryId > 0;
  readonly selectedImage = signal<File | null>(null);
  readonly imagePreviewUrl = signal<string | null>(null);
  readonly imageError = signal<string | null>(null);
  readonly isSaving = signal(false);
  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    slug: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    imageUrl: [''],
    displayOrder: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    if (this.isEditMode) {
      this.loadCategory();
    }
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    
    input.value = '';

    if (!file) {
      return;
    }

    this.imageError.set(null);

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];

    if (!allowedTypes.includes(file.type)) {
      this.imageError.set('فقط تصاویر JPG، PNG و WebP مجاز هستند.');

      return;
    }

    const maximumFileSize = 5 * 1024 * 1024;

    if (file.size > maximumFileSize) {
      this.imageError.set('حجم تصویر نباید بیشتر از ۵ مگابایت باشد.');

      return;
    }

    this.revokeSelectedPreview();

    this.selectedImage.set(file);

    this.imagePreviewUrl.set(URL.createObjectURL(file));
  }

  removeImage(): void {
    this.revokeSelectedPreview();

    this.selectedImage.set(null);
    this.imagePreviewUrl.set(null);

    this.form.controls.imageUrl.setValue('');
  }

  getImageSource(imageUrl: string): string {
    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
      return imageUrl;
    }

    const apiOrigin = environment.apiUrl.replace(/\/api\/?$/i, '');

    const normalizedImageUrl = imageUrl.startsWith('/') ? imageUrl : `/${imageUrl}`;

    return `${apiOrigin}${normalizedImageUrl}`;
  }

  submit(): void {
    if (this.form.invalid || this.isSaving()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.imageError.set(null);

    this.resolveImageUrl()
      .pipe(
        switchMap((imageUrl) => this.saveCategory(imageUrl)),

        finalize(() => {
          this.isSaving.set(false);
        }),
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/admin/categories']);
        },

        error: (error) => {
          console.error('Save category failed:', error);

          this.imageError.set('ذخیره دسته‌بندی یا آپلود تصویر با خطا مواجه شد.');
        },
      });
  }

  ngOnDestroy(): void {
    this.revokeSelectedPreview();
  }

  private loadCategory(): void {
    this.categoryService.getById(this.categoryId).subscribe({
      next: (category) => {
        this.form.patchValue({
          name: category.name,
          slug: category.slug,

          description: category.description ?? '',

          imageUrl: category.imageUrl ?? '',

          displayOrder: category.displayOrder,
        });

        if (category.imageUrl) {
          this.imagePreviewUrl.set(this.getImageSource(category.imageUrl));
        }
      },

      error: (error) => {
        console.error('Load category failed:', error);
      },
    });
  }

  private resolveImageUrl(): Observable<string | null> {
    const selectedImage = this.selectedImage();

    if (!selectedImage) {
      const currentImageUrl = this.form.controls.imageUrl.value.trim();

      return of(currentImageUrl || null);
    }

    return this.fileService
      .uploadCategoryImage(selectedImage)
      .pipe(map((response) => response.url));
  }

  private saveCategory(imageUrl: string | null): Observable<number | void> {
    const value = this.form.getRawValue();

    const request = {
      name: value.name.trim(),

      slug: value.slug.trim(),

      description: value.description.trim() || null,

      imageUrl,

      displayOrder: Number(value.displayOrder),
    };

    if (this.isEditMode) {
      return this.categoryService.update(this.categoryId, request as UpdateCategoryRequest);
    }

    return this.categoryService.create(request as CreateCategoryRequest);
  }

  private revokeSelectedPreview(): void {
    if (!this.selectedImage()) {
      return;
    }

    const previewUrl = this.imagePreviewUrl();

    if (previewUrl?.startsWith('blob:')) {
      URL.revokeObjectURL(previewUrl);
    }
  }
}
