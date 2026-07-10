import { Component, inject } from '@angular/core';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';
import { Seo } from '../../core/services/seo';

@Component({
  selector: 'app-products',
  imports: [ProductCard],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products {
  private readonly seo = inject(Seo);

  readonly products = MOCK_PRODUCTS;

  constructor() {
    this.seo.update(
      'محصولات | فروشگاه آنلاین',
      'مشاهده و خرید انواع محصولات با قیمت مناسب، ارسال سریع و پشتیبانی مطمئن.'
    );
  }
}