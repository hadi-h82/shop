import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';
import { Seo } from '../../core/services/seo';
const SITE_URL = 'https://example.com';

@Component({
  selector: 'app-home',
  imports: [RouterLink, MatButtonModule, MatIconModule, ProductCard],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

  private readonly seo = inject(Seo);

constructor() {
  this.seo.update({
    title: 'فروشگاه آنلاین | خرید آسان و سریع',
    description:
      'خرید اینترنتی محصولات متنوع با ارسال سریع و تجربه کاربری ساده.',
    canonicalUrl: `${SITE_URL}/`,
    type: 'website'
  });
}

  readonly featuredProducts = MOCK_PRODUCTS;
}