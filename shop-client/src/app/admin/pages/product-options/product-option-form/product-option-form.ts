import { HttpErrorResponse } from '@angular/common/http';

import { Component, inject, signal } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatIconModule } from '@angular/material/icon';

import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Observable } from 'rxjs';

import {
  AdminProductOptionDefinitionService,
  ProductOptionInputType,
} from '../../../services/admin-product-option-definition.service';

@Component({
  selector: 'app-admin-product-option-form',

  imports: [ReactiveFormsModule, RouterLink, MatIconModule, MatSnackBarModule],

  templateUrl: './product-option-form.html',
  styleUrl: './product-option-form.scss',
})
export class AdminProductOptionForm {
  private readonly fb = inject(FormBuilder);

  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly optionService = inject(AdminProductOptionDefinitionService);

  private readonly snackBar = inject(MatSnackBar);

  readonly definitionId = Number(this.route.snapshot.paramMap.get('id'));

  readonly isEditMode = this.definitionId > 0;

  readonly loading = signal(this.isEditMode);

  readonly submitting = signal(false);

  readonly inputTypes = [
    {
      value: ProductOptionInputType.Select,
      label: 'فهرست انتخابی',
    },
    {
      value: ProductOptionInputType.Radio,
      label: 'گزینه‌ای (رادیویی)',
    },
    {
      value: ProductOptionInputType.Color,
      label: 'رنگ',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],

    slug: [
      '',
      [
        Validators.required,
        Validators.maxLength(100),

        Validators.pattern(/^[a-z0-9]+(?:-[a-z0-9]+)*$/),
      ],
    ],

    inputType: [ProductOptionInputType.Select, [Validators.required]],

    displayOrder: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    if (this.isEditMode) {
      this.loadDefinition();
    }
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();

      return;
    }

    const value = this.form.getRawValue();

    const request = {
      name: value.name.trim(),

      slug: value.slug.trim().toLowerCase(),

      inputType: Number(value.inputType) as ProductOptionInputType,

      displayOrder: Number(value.displayOrder),
    };

    this.submitting.set(true);

    const request$: Observable<unknown> = this.isEditMode
      ? this.optionService.update(this.definitionId, request)
      : this.optionService.create(request);

    request$.subscribe({
      next: () => {
        this.router.navigate(['/admin/product-options']);
      },

      error: (error: HttpErrorResponse) => {
        console.error('Save product option definition failed:', error);

        this.submitting.set(false);

        const message =
          error.status === 409 ? 'این Slug قبلاً استفاده شده است.' : 'ذخیره ویژگی انجام نشد.';

        this.snackBar.open(message, 'بستن', {
          duration: 4000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        });
      },
    });
  }

  private loadDefinition(): void {
    this.optionService.getById(this.definitionId).subscribe({
      next: (definition) => {
        this.form.patchValue({
          name: definition.name,

          slug: definition.slug,

          inputType: definition.inputType,

          displayOrder: definition.displayOrder,
        });

        this.loading.set(false);
      },

      error: (error) => {
        console.error('Load product option definition failed:', error);

        this.loading.set(false);

        this.snackBar.open('ویژگی موردنظر پیدا نشد.', 'بستن', {
          duration: 4000,
        });
      },
    });
  }
}
