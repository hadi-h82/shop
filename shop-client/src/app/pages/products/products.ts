import {
  Component,
  computed,
  inject
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { Seo } from '../../core/services/seo';
import { ProductCard } from '../../shared/ui/product-card/product-card';

const SITE_URL = 'https://sevart.ir';

function normalizePersianText(value: string): string {
  return value
    .toLowerCase()
    .replace(/ي/g, 'ی')
    .replace(/ك/g, 'ک')
    .replace(/ۀ/g, 'ه')
    .replace(/ة/g, 'ه')
    .replace(/ؤ/g, 'و')
    .replace(/إ|أ/g, 'ا')
    .replace(/[\u064B-\u065F\u0670]/g, '')
    .replace(/\u200c/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

@Component({
  selector: 'app-products',
  imports: [
    RouterLink,
    ProductCard
  ],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products {
  private readonly seo = inject(Seo);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(
    this.route.queryParamMap,
    {
      initialValue: this.route.snapshot.queryParamMap
    }
  );

  readonly searchQuery = computed(() =>
    this.queryParams().get('q')?.trim() ?? ''
  );

  readonly products = computed(() => {
    const query = normalizePersianText(this.searchQuery());

    if (!query) {
      return MOCK_PRODUCTS;
    }

    return MOCK_PRODUCTS.filter(product => {
      const searchableText = normalizePersianText(
        `${product.title} ${product.categoryName}`
      );

      return searchableText.includes(query);
    });
  });

  constructor() {
    this.seo.update({
      title: 'محصولات | فروشگاه آنلاین',
      description:
        'مشاهده و خرید انواع محصولات با قیمت مناسب، ارسال سریع و پشتیبانی مطمئن.',
      canonicalUrl: `${SITE_URL}/products`,
      type: 'website'
    });
  }
}