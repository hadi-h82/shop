import {
  Component,
  inject
} from '@angular/core';

import { RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { DecimalPipe } from '@angular/common';
import {
  AdminProductListItem,
  AdminProductService
} from '../../../services/admin-product.service';

@Component({
  selector: 'app-admin-product-list',

  imports: [
    MatTableModule,
    MatIconModule,
    RouterLink,
    DecimalPipe
  ],

  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class AdminProductList {

  private readonly productService =
    inject(AdminProductService);

  readonly displayedColumns = [
    'name',
    'category',
    'price',
    'status',
    'actions'
  ];

  readonly products = toSignal(
    this.productService.getAll(),
    {
      initialValue: [] as AdminProductListItem[]
    }
  );

  getStatusLabel(status: number): string {
    switch (status) {
      case 0:
        return 'پیش‌نویس';

      case 1:
        return 'منتشر شده';

      case 2:
        return 'آرشیو شده';

      default:
        return 'نامشخص';
    }
  }
}