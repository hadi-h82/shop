import {
  Component,
  inject,
  signal
} from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CartService } from '../../core/services/cart/cart';



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

  private readonly expandedItems =
    signal<Record<string, boolean>>({});

  toggleOptions(cartItemId: string): void {
    this.expandedItems.update(currentItems => ({
      ...currentItems,
      [cartItemId]: !currentItems[cartItemId]
    }));
  }

  isOptionsExpanded(cartItemId: string): boolean {
    return Boolean(this.expandedItems()[cartItemId]);
  }
}