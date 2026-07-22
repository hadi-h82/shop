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
  },
  'finger-food-trays': {
    title: 'راهنمای انتخاب سینی فینگر فود',
    intro:
      'سینی فینگر فود باید با تعداد، ابعاد و شیوه چیدمان خوراکی‌ها هماهنگ باشد. انتخاب اندازه و فرم مناسب، جابه‌جایی و ارائه مرتب‌تر را آسان‌تر می‌کند.',
    tips: [
      'پیش از انتخاب، تعداد و اندازه تقریبی فینگر فودها را مشخص کنید.',
      'فرم سینی را متناسب با چیدمان میز و فضای سرو انتخاب کنید.',
      'برای جابه‌جایی مطمئن‌تر، به استحکام و سطح قابل‌استفاده سینی توجه کنید.'
    ],
    note:
      'اندازه و ویژگی‌های هر مدل را در صفحه محصول بررسی کنید تا انتخاب دقیق‌تری داشته باشید.'
  },
  plexiglass: {
    title: 'راهنمای انتخاب محصولات پلکسی',
    intro:
      'محصولات پلکسی در مدل‌های ساده و طرح‌دار برای چیدمان و تزئین استفاده می‌شوند. طرح، ابعاد و تناسب محصول با کاربرد موردنظر را پیش از انتخاب بررسی کنید.',
    tips: [
      'ابعاد محصول را با فضای چیدمان یا تزئین هماهنگ کنید.',
      'میان مدل ساده و طرح‌دار بر اساس سبک نهایی کار انتخاب کنید.',
      'جزئیات هر مدل را برای بررسی شکل و کاربرد آن مطالعه کنید.'
    ],
    note:
      'تصویر، ابعاد و ویژگی‌های ثبت‌شده در صفحه هر محصول را پیش از انتخاب بررسی کنید.'
  },
  boxes: {
    title: 'راهنمای انتخاب باکس',
    intro:
      'باکس مناسب باید با ابعاد، وزن و نوع چیدمان محصول هماهنگ باشد. انتخاب درست، ارائه منظم‌تر و جابه‌جایی مطمئن‌تر محتوا را فراهم می‌کند.',
    tips: [
      'ابعاد داخلی باکس را با اندازه محتوا مقایسه کنید.',
      'برای چیدمان چندتکه، فضای کافی بین اجزا در نظر بگیرید.',
      'مدل باکس را متناسب با نوع ارائه و جابه‌جایی انتخاب کنید.'
    ],
    note:
      'پیش از انتخاب، ابعاد و مشخصات درج‌شده برای هر باکس را بررسی کنید.'
  },
  stencils: {
    title: 'راهنمای انتخاب استنسیل',
    intro:
      'استنسیل مناسب به اجرای منظم‌تر طرح روی کیک و شیرینی کمک می‌کند. اندازه، جزئیات طرح و تناسب آن با سطح کار، مهم‌ترین موارد انتخاب هستند.',
    tips: [
      'ابعاد استنسیل را با سطحی که قرار است تزئین شود هماهنگ کنید.',
      'برای شروع، طرح‌هایی با جزئیات کمتر اجرای ساده‌تری دارند.',
      'طرح استنسیل را با سبک کلی تزئین کیک یا شیرینی هماهنگ کنید.'
    ],
    note:
      'پیش از خرید، تصویر طرح و ابعاد درج‌شده در صفحه محصول را بررسی کنید.'
  },
  'packaging-boxes': {
    title: 'راهنمای انتخاب جعبه',
    intro:
      'جعبه باید از نظر اندازه و فرم با کیک، شیرینی یا هدیه هماهنگ باشد. فضای کافی و انتخاب ابعاد درست، از فشرده‌شدن یا جابه‌جایی نامناسب محتوا جلوگیری می‌کند.',
    tips: [
      'ابعاد محصول را پیش از انتخاب جعبه به‌دقت اندازه‌گیری کنید.',
      'برای تزئینات برجسته، ارتفاع کافی داخل جعبه در نظر بگیرید.',
      'فرم جعبه را متناسب با نوع محصول و شیوه حمل انتخاب کنید.'
    ],
    note:
      'ابعاد داخلی و ویژگی‌های هر جعبه را در صفحه محصول با نیازتان تطبیق دهید.'
  },
  'pastry-tools': {
    title: 'راهنمای انتخاب ابزار و لوازم قنادی',
    intro:
      'انتخاب ابزار مناسب به نوع کار، میزان استفاده و مرحله پخت یا تزئین بستگی دارد. پیش از خرید، کاربرد و مشخصات هر ابزار را با نیازتان مقایسه کنید.',
    tips: [
      'ابتدا مشخص کنید ابزار برای پخت، آماده‌سازی یا تزئین موردنیاز است.',
      'ابعاد و شیوه استفاده ابزار را با فضای کار خود تطبیق دهید.',
      'برای استفاده مداوم، مدل مناسب با کاربرد اصلی خود را انتخاب کنید.'
    ],
    note:
      'توضیحات و ویژگی‌های هر ابزار را پیش از انتخاب نهایی بررسی کنید.'
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
