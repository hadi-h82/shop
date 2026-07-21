import {
  Component,
  computed,
  effect,
  inject
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { MOCK_CATEGORIES } from '../../core/mock-data/categories.mock';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { Seo } from '../../core/services/seo';
import { ProductCard } from '../../shared/ui/product-card/product-card';

const SITE_URL = 'https://sevart.ir';

interface CategoryGuide {
  title: string;
  intro: string;
  tips: string[];
  note: string;
}

const CATEGORY_GUIDES: Record<string, CategoryGuide> = {
  'cake-trays': {
    title: 'راهنمای انتخاب سینی کیک',
    intro:
      'سینی کیک علاوه بر زیباتر کردن ارائه، باید وزن کیک را بدون خم‌شدن تحمل کند. برای انتخاب بهتر، اندازه، جنس و ضخامت سینی را با نوع کیک هماهنگ کنید.',
    tips: [
      'سینی را حداقل ۴ تا ۶ سانتی‌متر بزرگ‌تر از قطر کیک انتخاب کنید.',
      'برای کیک‌های سنگین و چندطبقه، MDF ضخیم‌تر انتخاب مطمئن‌تری است.',
      'مدل دورو برای ارائه تمیزتر و استفاده دوباره مناسب‌تر است.'
    ],
    note:
      'اندازه و ویژگی‌های محصول را در صفحه هر مدل انتخاب کنید تا قیمت نهایی نمایش داده شود.'
  }
};

@Component({
  selector: 'app-category-products',
  imports: [
    RouterLink,
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

  readonly guide = computed<CategoryGuide | undefined>(() => {
    const category = this.category();

    if (!category) {
      return undefined;
    }

    return (
      CATEGORY_GUIDES[category.slug] ?? {
        title: `راهنمای خرید ${category.name}`,
        intro: category.description,
        tips: [
          'قبل از خرید، اندازه و جنس موردنیازتان را بررسی کنید.',
          'توضیحات و ویژگی‌های هر محصول را با مدل‌های دیگر مقایسه کنید.'
        ],
        note:
          'با انتخاب ویژگی‌های هر محصول، قیمت نهایی آن نمایش داده می‌شود.'
      }
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
        title:
          'دسته‌بندی پیدا نشد | فروشگاه لوازم قنادی',
        description:
          'دسته‌بندی موردنظر در فروشگاه پیدا نشد.',
        canonicalUrl:
          `${SITE_URL}/categories/${this.slug() ?? ''}`,
        type: 'website'
      });
    });
  }
}