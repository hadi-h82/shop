import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { ProductCard } from '../../shared/ui/product-card/product-card';

@Component({
  selector: 'app-home',
  imports: [RouterLink, MatButtonModule, MatIconModule,ProductCard],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

   readonly featuredProducts = MOCK_PRODUCTS;
}