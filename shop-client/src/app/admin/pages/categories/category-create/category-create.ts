import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';

import { AdminCategoryService } from '../../../services/admin-category.service';

@Component({
  selector: 'app-admin-category-create',
  imports: [ReactiveFormsModule, RouterLink, MatIconModule],
  templateUrl: './category-create.html',
  styleUrl: './category-create.scss',
})
export class AdminCategoryCreate {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly categoryService = inject(AdminCategoryService);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    slug: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    imageUrl: [''],
    displayOrder: [0, [Validators.required, Validators.min(0)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    this.categoryService
      .create({
        name: value.name.trim(),
        slug: value.slug.trim(),
        description: value.description.trim() || null,
        imageUrl: value.imageUrl.trim() || null,
        displayOrder: Number(value.displayOrder),
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/admin/categories']);
        },

        error: (error) => {
          console.error('Create category failed:', error);
        },
      });
  }
}
