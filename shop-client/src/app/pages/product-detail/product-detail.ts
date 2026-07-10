import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { Seo } from '../../core/services/seo';

@Component({
  selector: 'app-product-detail',
  imports: [
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly seo = inject(Seo);

  readonly slug = this.route.snapshot.paramMap.get('id');

  readonly product = computed(() =>
    MOCK_PRODUCTS.find(product => product.slug === this.slug)
  );

  constructor() {
    const product = this.product();

    if (product) {
      this.seo.update(
        `${product.title} | فروشگاه آنلاین`,
        `خرید ${product.title} از دسته‌بندی ${product.categoryName} با قیمت مناسب و ارسال سریع.`
      );
    } else {
      this.seo.update(
        'محصول پیدا نشد | فروشگاه آنلاین',
        'محصول موردنظر شما در فروشگاه پیدا نشد.'
      );
    }
  }
}