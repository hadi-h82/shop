import {
  Component,
  inject,
  signal
} from '@angular/core';

import {
  DecimalPipe
} from '@angular/common';

import {
  HttpErrorResponse
} from '@angular/common/http';

import {
  RouterLink
} from '@angular/router';

import {
  MatTableModule
} from '@angular/material/table';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  MatDialog
} from '@angular/material/dialog';

import {
  MatSnackBar,
  MatSnackBarModule
} from '@angular/material/snack-bar';

import {
  AdminProductListItem,
  AdminProductService,
  ProductStatus
} from '../../../services/admin-product.service';

import {
  ConfirmDialog
} from '../../../../shared/components/confirm-dialog/confirm-dialog';


@Component({
  selector: 'app-admin-product-list',

  imports: [
    MatTableModule,
    MatIconModule,
    RouterLink,
    DecimalPipe,
    MatSnackBarModule
  ],

  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class AdminProductList {

  // برای استفاده از Enum داخل HTML
  readonly ProductStatus = ProductStatus;


  private readonly productService =
    inject(AdminProductService);

  private readonly dialog =
    inject(MatDialog);

  private readonly snackBar =
    inject(MatSnackBar);


  readonly displayedColumns = [
    'name',
    'category',
    'price',
    'status',
    'actions'
  ];


  readonly products =
    signal<AdminProductListItem[]>([]);


  readonly loading =
    signal(false);


  readonly processingId =
    signal<number | null>(null);


  constructor() {
    this.loadProducts();
  }


  loadProducts(): void {

    this.loading.set(true);

    this.productService
      .getAll()
      .subscribe({

        next: products => {

          this.products.set(
            [...products].sort(
              (first, second) =>
                first.displayOrder -
                second.displayOrder
            )
          );

          this.loading.set(false);
        },


        error: error => {

          console.error(
            'Load products failed:',
            error
          );

          this.loading.set(false);

          this.showMessage(
            'خطا در دریافت محصولات.'
          );
        }

      });
  }


  getStatusLabel(
    status: ProductStatus
  ): string {

    switch (status) {

      case ProductStatus.Draft:
        return 'پیش‌نویس';

      case ProductStatus.Published:
        return 'منتشر شده';

      case ProductStatus.Archived:
        return 'آرشیو شده';

      default:
        return 'نامشخص';
    }
  }


  toggleStatus(
    product: AdminProductListItem
  ): void {

    if (this.processingId() !== null) {
      return;
    }


    this.processingId.set(
      product.id
    );


    const isPublished =
      product.status ===
      ProductStatus.Published;


    const request$ =
      isPublished
        ? this.productService
            .deactivate(product.id)
        : this.productService
            .activate(product.id);


    request$.subscribe({

      next: () => {

        const newStatus =
          isPublished
            ? ProductStatus.Archived
            : ProductStatus.Published;


        this.products.update(
          products =>
            products.map(item =>
              item.id === product.id
                ? {
                    ...item,
                    status: newStatus
                  }
                : item
            )
        );


        this.processingId.set(null);


        this.showMessage(
          isPublished
            ? 'محصول غیرفعال شد.'
            : 'محصول منتشر شد.'
        );
      },


      error: error => {

        console.error(
          'Change product status failed:',
          error
        );

        this.processingId.set(null);

        this.showMessage(
          'تغییر وضعیت محصول انجام نشد.'
        );
      }

    });
  }


  deleteProduct(
    product: AdminProductListItem
  ): void {

    if (this.processingId() !== null) {
      return;
    }


    const dialogRef =
      this.dialog.open(
        ConfirmDialog,
        {
          width: '420px',

          maxWidth:
            'calc(100vw - 32px)',

          autoFocus: false,

          data: {
            title: 'حذف محصول',

            message:
              `آیا از حذف محصول «${product.name}» مطمئن هستید؟`,

            confirmText:
              'حذف محصول',

            cancelText:
              'انصراف',

            type: 'danger'
          }
        }
      );


    dialogRef
      .afterClosed()
      .subscribe(
        (confirmed: boolean) => {

          if (!confirmed) {
            return;
          }


          this.processingId.set(
            product.id
          );


          this.productService
            .delete(product.id)
            .subscribe({

              next: () => {

                this.products.update(
                  products =>
                    products.filter(
                      item =>
                        item.id !==
                        product.id
                    )
                );


                this.processingId.set(null);


                this.showMessage(
                  'محصول با موفقیت حذف شد.'
                );
              },


              error: (
                error: HttpErrorResponse
              ) => {

                console.error(
                  'Delete product failed:',
                  error
                );


                this.processingId.set(null);


                // برای زمانی که OrderItem
                // پیاده‌سازی شد
                if (error.status === 409) {

                  const message =
                    error.error?.message ??
                    'این محصول در لیست سفارشات وجود دارد و امکان حذف آن وجود ندارد.';

                  this.showMessage(
                    message
                  );

                  return;
                }


                if (error.status === 404) {

                  this.showMessage(
                    'محصول پیدا نشد.'
                  );

                  return;
                }


                this.showMessage(
                  'حذف محصول انجام نشد.'
                );
              }

            });

        }
      );
  }


  private showMessage(
    message: string
  ): void {

    this.snackBar.open(
      message,
      'بستن',
      {
        duration: 4000,

        horizontalPosition:
          'center',

        verticalPosition:
          'bottom'
      }
    );
  }
}