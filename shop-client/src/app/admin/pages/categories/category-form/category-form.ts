import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { AdminCategoryService } from '../../../services/admin-category.service';

@Component({
  selector: 'app-admin-category-form',
  imports: [ReactiveFormsModule, RouterLink, MatIconModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss',
})
export class AdminCategoryForm {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly categoryService = inject(AdminCategoryService);
  private readonly route = inject(ActivatedRoute);



  readonly categoryId = Number(
  this.route.snapshot.paramMap.get('id')
  );

  readonly isEditMode = this.categoryId > 0;

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

private loadCategory(): void {
  this.categoryService
    .getById(this.categoryId)
    .subscribe({
      next: category => {
        this.form.patchValue({
          name: category.name,
          slug: category.slug,
          description: category.description ?? '',
          imageUrl: category.imageUrl ?? '',
          displayOrder: category.displayOrder
        });
      },

      error: error => {
        console.error(
          'Load category failed:',
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

  const request = {
    name: value.name.trim(),
    slug: value.slug.trim(),

    description:
      value.description.trim() || null,

    imageUrl:
      value.imageUrl.trim() || null,

    displayOrder:
      Number(value.displayOrder)
  };


  // Edit
  if (this.isEditMode) {
    this.categoryService
      .update(
        this.categoryId,
        request
      )
      .subscribe({
        next: () => {
          this.router.navigate([
            '/admin/categories'
          ]);
        },

        error: error => {
          console.error(
            'Update category failed:',
            error
          );
        }
      });

    return;
  }


  // Create
  this.categoryService
    .create(request)
    .subscribe({
      next: () => {
        this.router.navigate([
          '/admin/categories'
        ]);
      },

      error: error => {
        console.error(
          'Create category failed:',
          error
        );
      }
    });
}
}
