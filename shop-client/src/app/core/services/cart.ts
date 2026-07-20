import { isPlatformBrowser } from '@angular/common';
import {
  computed,
  effect,
  inject,
  Injectable,
  PLATFORM_ID,
  signal
} from '@angular/core';

import { CartItem } from '../models/cart-item.model';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly storageKey = 'shop-cart';

  private readonly itemsState = signal<CartItem[]>(
    this.loadInitialCart()
  );

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

  constructor() {
    effect(() => {
      if (!isPlatformBrowser(this.platformId)) {
        return;
      }

      localStorage.setItem(
        this.storageKey,
        JSON.stringify(this.itemsState())
      );
    });
  }

  add(product: Product): void {
    if (!product.isAvailable || product.price <= 0) {
      return;
    }
    const existingItem = this.itemsState().find(
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

  private loadInitialCart(): CartItem[] {
    if (!isPlatformBrowser(this.platformId)) {
      return [];
    }

    try {
      const storedCart = localStorage.getItem(this.storageKey);

      if (!storedCart) {
        return [];
      }

      const parsedCart = JSON.parse(storedCart);

      return Array.isArray(parsedCart) ? parsedCart : [];
    } catch {
      return [];
    }
  }
}