import { Component, inject, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import {
  MatSnackBar,
  MatSnackBarModule,
} from '@angular/material/snack-bar';
import { RouterLink } from '@angular/router';

import {
  AdminProductOptionDefinitionResponse,
  AdminProductOptionDefinitionService,
  ProductOptionInputType,
} from '../../../services/admin-product-option-definition.service';

@Component({
  selector: 'app-admin-product-option-list',

  imports: [
    RouterLink,
    MatIconModule,
    MatSnackBarModule,
  ],

  templateUrl: './product-option-list.html',
  styleUrl: './product-option-list.scss',
})
export class AdminProductOptionList {
  private readonly optionService =
    inject(AdminProductOptionDefinitionService);

  private readonly snackBar =
    inject(MatSnackBar);

  readonly definitions =
    signal<AdminProductOptionDefinitionResponse[]>([]);

  readonly loading = signal(true);

  readonly processingId =
    signal<number | null>(null);

  constructor() {
    this.loadDefinitions();
  }

  loadDefinitions(): void {
    this.loading.set(true);

    this.optionService
      .getAll()
      .subscribe({
        next: definitions => {
          this.definitions.set(
            [...definitions].sort(
              (first, second) =>
                first.displayOrder -
                second.displayOrder,
            ),
          );

          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Load product option definitions failed:',
            error,
          );

          this.loading.set(false);

          this.showMessage(
            'خطا در دریافت ویژگی‌ها.',
          );
        },
      });
  }

  toggleStatus(
    definition:
      AdminProductOptionDefinitionResponse,
  ): void {
    if (this.processingId() !== null) {
      return;
    }

    this.processingId.set(definition.id);

    const request$ = definition.isActive
      ? this.optionService.deactivate(
          definition.id,
        )
      : this.optionService.activate(
          definition.id,
        );

    request$.subscribe({
      next: () => {
        this.definitions.update(
          definitions =>
            definitions.map(item =>
              item.id === definition.id
                ? {
                    ...item,
                    isActive:
                      !item.isActive,
                  }
                : item,
            ),
        );

        this.processingId.set(null);

        this.showMessage(
          definition.isActive
            ? 'ویژگی غیرفعال شد.'
            : 'ویژگی فعال شد.',
        );
      },

      error: error => {
        console.error(
          'Change product option status failed:',
          error,
        );

        this.processingId.set(null);

        this.showMessage(
          'تغییر وضعیت ویژگی انجام نشد.',
        );
      },
    });
  }

  inputTypeLabel(
    inputType: ProductOptionInputType,
  ): string {
    switch (inputType) {
      case ProductOptionInputType.Radio:
        return 'گزینه‌ای';

      case ProductOptionInputType.Color:
        return 'رنگ';

      default:
        return 'فهرست انتخابی';
    }
  }

  private showMessage(
    message: string,
  ): void {
    this.snackBar.open(
      message,
      'بستن',
      {
        duration: 4000,
        horizontalPosition: 'center',
        verticalPosition: 'bottom',
      },
    );
  }
}