import { Component, inject } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Router, RouterLink } from '@angular/router';

import { toSignal } from '@angular/core/rxjs-interop';

import { catchError, of } from 'rxjs';

import { MatIconModule } from '@angular/material/icon';



import { AdminProductService, CreateAdminProductRequest } from '../../../services/admin-product.service';
import { CategoryService } from '../../../../core/services/category/category.service';

@Component({
  selector: 'app-admin-product-create',

  imports: [ReactiveFormsModule, RouterLink, MatIconModule],

  templateUrl: './product-create.html',
  styleUrl: './product-create.scss',
})
export class AdminProductCreate {
  private readonly fb = inject(FormBuilder);

  private readonly router = inject(Router);

  private readonly categoryService = inject(CategoryService);

  private readonly productService = inject(AdminProductService);

  readonly categories = toSignal(this.categoryService.getAll().pipe(catchError(() => of([]))), {
    initialValue: [],
  });

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],

    slug: ['', [Validators.required, Validators.maxLength(200)]],

    categoryId: [0, [Validators.required, Validators.min(1)]],

    description: [''],

    price: [0, [Validators.required, Validators.min(0)]],

    displayOrder: [0, [Validators.required, Validators.min(0)]],
  });

submit(): void {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  const value = this.form.getRawValue();

  const request: CreateAdminProductRequest = {
    categoryId: Number(value.categoryId),
    name: value.name.trim(),
    slug: value.slug.trim(),
    description: value.description?.trim() || null,
    price: Number(value.price),
    displayOrder: Number(value.displayOrder),

    // Options را قدم بعدی به فرم اضافه می‌کنیم
    options: []
  };

  console.log('Create product request:', request);

  this.productService
    .create(request)
    .subscribe({
      next: response => {
        console.log('Product created:', response);

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
