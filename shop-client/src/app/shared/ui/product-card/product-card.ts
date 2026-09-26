import {
  Component,
  inject,
  input,
} from '@angular/core';

import {
  RouterLink,
} from '@angular/router';

import {
  MatButtonModule,
} from '@angular/material/button';

import {
  MatIconModule,
} from '@angular/material/icon';

import {
  Product,
} from '../../../core/models/product.model';

import {
  MediaUrlService,
} from '../../../core/services/media-url/media-url.service';

@Component({
  selector: 'app-product-card',

  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
  ],

  templateUrl: './product-card.html',
  styleUrl: './product-card.scss',
})
export class ProductCard {
  readonly product =
    input.required<Product>();

  readonly mediaUrl =
    inject(MediaUrlService);
}