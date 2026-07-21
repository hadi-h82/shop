import {
  Component,
  computed,
  effect,
  inject
} from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { MOCK_CATEGORIES } from '../../core/mock-data/categories.mock';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { Seo } from '../../core/services/seo';
import { ProductCard } from '../../shared/ui/product-card/product-card';

const SITE_URL = 'https://sevart.ir';

@Component({
  selector: 'app-category-products',
  imports: [
    RouterLink,
    NgOptimizedImage,
    ProductCard
  ],
  templateUrl: './category-products.html',
  styleUrl: './category-products.scss'
})
export class CategoryProducts {
  private readonly route = inject(ActivatedRoute);
  private readonly seo = inject(Seo);

  private readonly routeParams = toSignal(
    this.route.paramMap,
    {
      initialValue: this.route.snapshot.paramMap
    }
  );

  readonly slug = computed(() =>
    this.routeParams().get('slug')
  );

  readonly category = computed(() => {
    const slug = this.slug();

    return MOCK_CATEGORIES.find(
      category =>
        category.slug === slug &&
        category.isActive
    );
  });

  readonly products = computed(() => {
    const category = this.category();

    if (!category) {
      return [];
    }

    return MOCK_PRODUCTS.filter(
      product => product.categoryId === category.id
    );
  });

  constructor() {
    effect(() => {
      const category = this.category();

      if (category) {
        this.seo.update({
          title: `${category.name} | فروشگاه لوازم قنادی`,
          description: category.description,
          canonicalUrl:
            `${SITE_URL}/categories/${category.slug}`,
          imageUrl:
            `${SITE_URL}${category.imageUrl}`,
          type: 'website'
        });

        return;
      }

      this.seo.update({
        title: 'دسته‌بندی پیدا نشد | فروشگاه لوازم قنادی',
        description:
          'دسته‌بندی موردنظر در فروشگاه پیدا نشد.',
        canonicalUrl: `${SITE_URL}/categories/${this.slug() ?? ''}`,
        type: 'website'
      });
    });
  }
}