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
import {
  isPlatformBrowser,
  NgOptimizedImage
} from '@angular/common';
import { MOCK_CATEGORIES } from '../../core/mock-data/categories.mock';


const SITE_URL = 'https://sevart.ir';

interface HeroSlide {
  title: string;
  description: string;
  image: string;
  link: string;
}

@Component({
  selector: 'app-home',
  imports: [
  RouterLink,
  MatButtonModule,
  MatIconModule,
  NgOptimizedImage,
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


readonly categories = MOCK_CATEGORIES
  .filter(category => category.isActive)
  .sort((first, second) => first.displayOrder - second.displayOrder);

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