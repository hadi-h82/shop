import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';
import { Seo } from '../../core/services/seo';
import {
  Component,
  DestroyRef,
  inject,
  OnInit,
  PLATFORM_ID,
  signal
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';


const SITE_URL = 'https://example.com';

interface HeroSlide {
  title: string;
  description: string;
  image: string;
  link: string;
}

interface HomeCategory {
  title: string;
  subtitle: string;
  image: string;
  link: string;
}

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    ProductCard
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit {
  private readonly seo = inject(Seo);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly destroyRef = inject(DestroyRef);

  private sliderInterval?: ReturnType<typeof setInterval>;

  readonly activeSlide = signal(0);

  readonly slides: HeroSlide[] = [
    {
      title: 'هر چیزی برای خلق شیرینی‌های خاص',
      description:
        'انواع ابزار قنادی، قالب، تجهیزات پخت و لوازم حرفه‌ای برای شیرینی‌پزی.',
      image: '/images/home/slide-tools.webp',
      link: '/products'
    },
    {
      title: 'تزئین حرفه‌ای، نتیجه‌ای چشم‌نواز',
      description:
        'ابزارهای تخصصی تزئین کیک و شیرینی برای تبدیل هر ایده به یک اثر زیبا.',
      image: '/images/home/slide-decoration.webp',
      link: '/products'
    },
    {
      title: 'از پخت تا بسته‌بندی، همه‌چیز یکجا',
      description:
        'محصولات موردنیاز قنادی‌ها، شیرینی‌پزها و علاقه‌مندان به هنر شیرینی‌پزی.',
      image: '/images/home/slide-packaging.webp',
      link: '/products'
    }
  ];


readonly categories: HomeCategory[] = [
  {
    title: 'قالب و ابزار پخت',
    subtitle: 'قالب، وردنه، همزن و ابزارهای ضروری',
    image: '/images/home/category-baking-tools.webp',
    link: '/products'
  },
  {
    title: 'ابزار تزئین',
    subtitle: 'قیف، ماسوره، کاردک و لوازم تزئین',
    image: '/images/home/category-decoration.webp',
    link: '/products'
  },
  {
    title: 'مواد اولیه',
    subtitle: 'مواد موردنیاز برای پخت و شیرینی‌پزی',
    image: '/images/home/category-ingredients.webp',
    link: '/products'
  },
  {
    title: 'رنگ و اسانس',
    subtitle: 'رنگ‌های خوراکی و طعم‌دهنده‌های متنوع',
    image: '/images/home/category-colors.webp',
    link: '/products'
  },
  {
    title: 'بسته‌بندی',
    subtitle: 'جعبه کیک و شیرینی برای ارائه‌ای زیباتر',
    image: '/images/home/category-packaging.webp',
    link: '/products'
  },
  {
    title: 'تجهیزات حرفه‌ای',
    subtitle: 'تجهیزات مناسب قنادی‌ها و کارگاه‌ها',
    image: '/images/home/category-equipment.webp',
    link: '/products'
  }
];

  readonly featuredProducts = MOCK_PRODUCTS;

  constructor() {
    this.seo.update({
      title: 'فروشگاه لوازم قنادی و شیرینی‌پزی',
      description:
        'خرید انواع ابزار قنادی، تجهیزات پخت، لوازم تزئین کیک و مواد اولیه شیرینی‌پزی.',
      canonicalUrl: `${SITE_URL}/`,
      type: 'website'
    });

    this.destroyRef.onDestroy(() => {
      this.stopAutoPlay();
    });
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.startAutoPlay();
    }
  }

  nextSlide(resetTimer = true): void {
    this.activeSlide.update(
      current => (current + 1) % this.slides.length
    );

    if (resetTimer) {
      this.restartAutoPlay();
    }
  }

  previousSlide(): void {
    this.activeSlide.update(
      current => (current - 1 + this.slides.length) % this.slides.length
    );

    this.restartAutoPlay();
  }

  goToSlide(index: number): void {
    this.activeSlide.set(index);
    this.restartAutoPlay();
  }

  pauseAutoPlay(): void {
    this.stopAutoPlay();
  }

  resumeAutoPlay(): void {
    this.startAutoPlay();
  }

  private startAutoPlay(): void {
    this.stopAutoPlay();

    this.sliderInterval = setInterval(() => {
      this.nextSlide(false);
    }, 4000);
  }

  private stopAutoPlay(): void {
    if (this.sliderInterval) {
      clearInterval(this.sliderInterval);
      this.sliderInterval = undefined;
    }
  }

  private restartAutoPlay(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.startAutoPlay();
    }
  }
}