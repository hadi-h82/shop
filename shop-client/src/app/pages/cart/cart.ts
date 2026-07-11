import { Component, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { CartService } from '../../core/services/cart';

@Component({
  selector: 'app-cart',
  imports: [
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class Cart {
  readonly cart = inject(CartService);
}