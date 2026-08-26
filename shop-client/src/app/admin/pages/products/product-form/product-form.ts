import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { AdminProductService, CreateAdminProductRequest } from '../../../services/admin-product.service';
import { CategoryService } from '../../../../core/services/category/category.service';

import {
  UpdateAdminProductRequest
} from '../../../services/admin-product.service';

@Component({
  selector: 'app-admin-product-form',

  imports: [ReactiveFormsModule, RouterLink, MatIconModule],

  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class AdminProductForm {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(AdminProductService);
  private readonly route = inject(ActivatedRoute);

  readonly categories = toSignal(this.categoryService.getAll().pipe(catchError(() => of([]))), {
    initialValue: [],
  });

  readonly productId = Number(
  this.route.snapshot.paramMap.get('id')
);

readonly isEditMode =
  this.productId > 0;

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],

    slug: ['', [Validators.required, Validators.maxLength(200)]],

    categoryId: [0, [Validators.required, Validators.min(1)]],

    description: [''],

    price: [0, [Validators.required, Validators.min(0)]],

    displayOrder: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
  if (this.isEditMode) {
    this.loadProduct();
  }
}

private loadProduct(): void {
  this.productService
    .getById(this.productId)
    .subscribe({
      next: product => {
        this.form.patchValue({
          name: product.name,
          slug: product.slug,
          categoryId: product.categoryId,
          description: product.description ?? '',
          price: product.price,
          displayOrder: product.displayOrder
        });
      },

      error: error => {
        console.error(
          'Load product failed:',
          error
        );
      }
    });
}

submit(): void {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  const value = this.form.getRawValue();

  const request: UpdateAdminProductRequest = {
    categoryId: Number(value.categoryId),
    name: value.name.trim(),
    slug: value.slug.trim(),
    description: value.description?.trim() || null,
    price: Number(value.price),
    displayOrder: Number(value.displayOrder)
  };

  // =========================
  // Edit
  // =========================

  if (this.isEditMode) {
    this.productService
      .update(
        this.productId,
        request
      )
      .subscribe({
        next: () => {
          this.router.navigate([
            '/admin/products'
          ]);
        },

        error: error => {
          console.error(
            'Update product failed:',
            error
          );
        }
      });

    return;
  }

  // =========================
  // Create
  // =========================

  const createRequest: CreateAdminProductRequest = {
    ...request,

    // فعلاً Options جداگانه مدیریت می‌شود
    options: []
  };

  this.productService
    .create(createRequest)
    .subscribe({
      next: response => {
        console.log(
          'Product created:',
          response
        );

        this.router.navigate([
          '/admin/products'
        ]);
      },

      error: error => {
        console.error(
          'Create product failed:',
          error
        );
      }
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
    input.value = this.formatPrice(
      this.form.controls.price.value
    );
    return;
  }

  const numericValue =
    rawValue === '' ? 0 : Number(rawValue);

  this.form.controls.price.setValue(
    numericValue
  );

  input.value = rawValue
    ? numericValue.toLocaleString('en-US')
    : '';
}
}
