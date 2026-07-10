import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';

@Component({
  selector: 'app-product-detail',
  imports: [DecimalPipe, RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetail {
  private readonly route = inject(ActivatedRoute);

  readonly slug = this.route.snapshot.paramMap.get('id');

  readonly product = computed(() =>
    MOCK_PRODUCTS.find(product => product.slug === this.slug)
  );
}