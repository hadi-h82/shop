import { Component, input } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-card',
imports: [
  DecimalPipe,
  RouterLink,
  MatButtonModule,
  MatIconModule
],
  templateUrl: './product-card.html',
  styleUrl: './product-card.scss'
})
export class ProductCard {
  product = input.required<Product>();
}