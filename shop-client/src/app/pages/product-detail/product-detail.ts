import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CartService } from '../../core/services/cart'; 
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { Seo } from '../../core/services/seo';
const SITE_URL = 'https://sevart.ir';

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

  private readonly cart = inject(CartService);
  private readonly route = inject(ActivatedRoute);
  private readonly seo = inject(Seo);

  readonly slug = this.route.snapshot.paramMap.get('id');

  readonly product = computed(() =>
    MOCK_PRODUCTS.find(product => product.slug === this.slug)
  );


  addToCart(): void {
  const product = this.product();

  if (!product || !product.isAvailable) {
    return;
  }

  this.cart.add(product);
}

onImageError(event: Event): void {
  const image = event.target;

  if (
    !(image instanceof HTMLImageElement) ||
    image.dataset['fallbackApplied'] === 'true'
  ) {
    return;
  }

  image.dataset['fallbackApplied'] = 'true';
  image.src = '/images/home/default-product-image.webp';
}

constructor() {
  const product = this.product();

  if (product) {
    this.seo.update({
      title: `${product.title} | فروشگاه آنلاین`,
      description:
        `خرید ${product.title} از دسته‌بندی ${product.categoryName} با قیمت مناسب و ارسال سریع.`,
      canonicalUrl: `${SITE_URL}/products/${product.slug}`,
      imageUrl: `${SITE_URL}${product.imageUrl}`,
      type: 'product'
    });
  } else {
    this.seo.update({
      title: 'محصول پیدا نشد | فروشگاه آنلاین',
      description: 'محصول موردنظر شما در فروشگاه پیدا نشد.',
      canonicalUrl: `${SITE_URL}/products/${this.slug}`,
      type: 'website'
    });
  }
}
}
