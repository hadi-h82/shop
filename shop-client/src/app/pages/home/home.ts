import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';
import { Seo } from '../../core/services/seo';

const SITE_URL = 'https://example.com';

interface HeroSlide {
  title: string;
  description: string;
  image: string;
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
export class Home {

  private readonly seo = inject(Seo);

  readonly featuredProducts = MOCK_PRODUCTS;

  readonly slides: HeroSlide[] = [
    {
      title: 'هر چیزی برای خلق شیرینی‌های خاص',
      description:
        'انواع ابزار قنادی، قالب، تجهیزات پخت و لوازم حرفه‌ای برای شیرینی‌پزی.',
      image: '/images/home/slide-tools.webp'
    },
    {
      title: 'تزئین حرفه‌ای، نتیجه‌ای چشم‌نواز',
      description:
        'ابزارهای تخصصی تزئین کیک و شیرینی برای تبدیل هر ایده به یک اثر زیبا.',
      image: '/images/home/slide-decoration.webp'
    },
    {
      title: 'از پخت تا بسته‌بندی، همه‌چیز یکجا',
      description:
        'محصولات موردنیاز قنادی‌ها، شیرینی‌پزها و علاقه‌مندان به هنر شیرینی‌پزی.',
      image: '/images/home/slide-packaging.webp'
    }
  ];

  readonly activeSlide = signal(0);

  constructor() {
    this.seo.update({
      title: 'فروشگاه لوازم قنادی و شیرینی‌پزی',
      description:
        'خرید انواع ابزار قنادی، تجهیزات پخت، لوازم تزئین کیک و مواد اولیه شیرینی‌پزی.',
      canonicalUrl: `${SITE_URL}/`,
      type: 'website'
    });
  }

  nextSlide(): void {
    this.activeSlide.update(
      current => (current + 1) % this.slides.length
    );
  }

  previousSlide(): void {
    this.activeSlide.update(
      current => (current - 1 + this.slides.length) % this.slides.length
    );
  }

  goToSlide(index: number): void {
    this.activeSlide.set(index);
  }
}