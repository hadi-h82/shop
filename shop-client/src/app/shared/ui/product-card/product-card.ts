import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-card',
imports: [
  RouterLink,
  MatButtonModule,
  MatIconModule
],
  templateUrl: './product-card.html',
  styleUrl: './product-card.scss'
})
export class ProductCard {
  product = input.required<Product>();

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
}
