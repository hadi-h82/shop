import { computed, Injectable, signal } from '@angular/core';

import { CartItem } from '../models/cart-item.model';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class CartService  {
  private readonly itemsState = signal<CartItem[]>([]);

  readonly items = this.itemsState.asReadonly();

  readonly totalItems = computed(() =>
    this.itemsState().reduce(
      (total, item) => total + item.quantity,
      0
    )
  );

  readonly totalPrice = computed(() =>
    this.itemsState().reduce((total, item) => {
      const unitPrice =
        item.product.discountPrice ?? item.product.price;

      return total + unitPrice * item.quantity;
    }, 0)
  );

  add(product: Product): void {
    const currentItems = this.itemsState();
    const existingItem = currentItems.find(
      item => item.product.id === product.id
    );

    if (existingItem) {
      this.itemsState.update(items =>
        items.map(item =>
          item.product.id === product.id
            ? { ...item, quantity: item.quantity + 1 }
            : item
        )
      );

      return;
    }

    this.itemsState.update(items => [
      ...items,
      {
        product,
        quantity: 1
      }
    ]);
  }

  increase(productId: number): void {
    this.itemsState.update(items =>
      items.map(item =>
        item.product.id === productId
          ? { ...item, quantity: item.quantity + 1 }
          : item
      )
    );
  }

  decrease(productId: number): void {
    this.itemsState.update(items =>
      items
        .map(item =>
          item.product.id === productId
            ? { ...item, quantity: item.quantity - 1 }
            : item
        )
        .filter(item => item.quantity > 0)
    );
  }

  remove(productId: number): void {
    this.itemsState.update(items =>
      items.filter(item => item.product.id !== productId)
    );
  }

  clear(): void {
    this.itemsState.set([]);
  }
}