import { Component } from '@angular/core';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';

@Component({
  selector: 'app-products',
  imports: [ProductCard],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products {
  readonly products = MOCK_PRODUCTS;
}