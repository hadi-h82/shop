import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AdminCategoryService } from '../../../services/admin-category.service';
import { Category } from '../../../../core/models/category.model';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-admin-category-list',
  imports: [RouterLink, MatIconModule, MatSnackBarModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss',
})
export class AdminCategoryList {
  private readonly categoryService = inject(AdminCategoryService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);
  readonly processingId = signal<number | null>(null);

  constructor() {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories.set(
          categories.sort((first, second) => first.displayOrder - second.displayOrder),
        );

        this.loading.set(false);
      },

      error: (error) => {
        console.error('Load categories failed:', error);

        this.loading.set(false);

        this.showMessage('خطا در دریافت دسته‌بندی‌ها.');
      },
    });
  }

  toggleStatus(category: Category): void {
    if (this.processingId() !== null) {
      return;
    }

    this.processingId.set(category.id);

    const request$ = category.isActive
      ? this.categoryService.deactivate(category.id)
      : this.categoryService.activate(category.id);

    request$.subscribe({
      next: () => {
        this.categories.update((categories) =>
          categories.map((item) =>
            item.id === category.id
              ? {
                  ...item,
                  isActive: !category.isActive,
                }
              : item,
          ),
        );

        this.processingId.set(null);

        this.showMessage(category.isActive ? 'دسته‌بندی غیرفعال شد.' : 'دسته‌بندی فعال شد.');
      },

      error: (error) => {
        console.error('Change category status failed:', error);

        this.processingId.set(null);

        this.showMessage('تغییر وضعیت دسته‌بندی انجام نشد.');
      },
    });
  }

 deleteCategory(category: Category): void {

  if (this.processingId() !== null) {
    return;
  }

  const dialogRef = this.dialog.open(
    ConfirmDialog,
    {
      width: '420px',
      maxWidth: 'calc(100vw - 32px)',
      autoFocus: false,

      data: {
        title: 'حذف دسته‌بندی',
        message:
          `آیا از حذف دسته‌بندی «${category.name}» مطمئن هستید؟`,
        confirmText: 'حذف دسته‌بندی',
        cancelText: 'انصراف',
        type: 'danger'
      }
    }
  );

  dialogRef
    .afterClosed()
    .subscribe((confirmed: boolean) => {

      if (!confirmed) {
        return;
      }

      this.processingId.set(category.id);

      this.categoryService
        .delete(category.id)
        .subscribe({

          next: () => {

            this.categories.update(
              categories =>
                categories.filter(
                  item =>
                    item.id !== category.id
                )
            );

            this.processingId.set(null);

            this.showMessage(
              'دسته‌بندی با موفقیت حذف شد.'
            );
          },

          error: (
            error: HttpErrorResponse
          ) => {

            this.processingId.set(null);

            console.error(
              'Delete category failed:',
              error
            );

            if (error.status === 409) {

              const message =
                error.error?.message ??
                'این دسته‌بندی دارای محصول می‌باشد و امکان حذف آن وجود ندارد.';

              this.showMessage(message);

              return;
            }

            if (error.status === 404) {

              this.showMessage(
                'دسته‌بندی پیدا نشد.'
              );

              return;
            }

            this.showMessage(
              'حذف دسته‌بندی انجام نشد.'
            );
          }
        });

    });
}

  private showMessage(message: string): void {
    this.snackBar.open(message, 'بستن', {
      duration: 4000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }
}
