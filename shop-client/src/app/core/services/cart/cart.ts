import { isPlatformBrowser } from '@angular/common';
import {
  computed,
  effect,
  inject,
  Injectable,
  PLATFORM_ID,
  signal
} from '@angular/core';
import { CartItem, SelectedCartOption } from '../../models/cart-item.model';
import { Product } from '../../models/product.model';



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
    this.itemsState().reduce(
      (total, item) =>
        total + item.finalPrice * item.quantity,
      0
    )
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

  add(
    product: Product,
    selectedOptions: SelectedCartOption[] = [],
    finalPrice?: number,
    quantity = 1
  ): void {
    const calculatedFinalPrice =
      finalPrice ??
      product.discountPrice ??
      product.price;

    if (
      !product.isAvailable ||
      calculatedFinalPrice <= 0 ||
      quantity <= 0
    ) {
      return;
    }

    const cartItemId = this.createCartItemId(
      product.id,
      selectedOptions
    );

    const existingItem = this.itemsState().find(
      item => item.cartItemId === cartItemId
    );

    if (existingItem) {
      this.itemsState.update(items =>
        items.map(item =>
          item.cartItemId === cartItemId
            ? {
                ...item,
                quantity: item.quantity + quantity
              }
            : item
        )
      );

      return;
    }

    this.itemsState.update(items => [
      ...items,
      {
        cartItemId,
        product,
        quantity,
        selectedOptions,
        finalPrice: calculatedFinalPrice
      }
    ]);
  }

  increase(cartItemId: string): void {
    this.itemsState.update(items =>
      items.map(item =>
        item.cartItemId === cartItemId
          ? {
              ...item,
              quantity: item.quantity + 1
            }
          : item
      )
    );
  }

  decrease(cartItemId: string): void {
    this.itemsState.update(items =>
      items
        .map(item =>
          item.cartItemId === cartItemId
            ? {
                ...item,
                quantity: item.quantity - 1
              }
            : item
        )
        .filter(item => item.quantity > 0)
    );
  }

  remove(cartItemId: string): void {
    this.itemsState.update(items =>
      items.filter(
        item => item.cartItemId !== cartItemId
      )
    );
  }

  clear(): void {
    this.itemsState.set([]);
  }

  private createCartItemId(
    productId: number,
    selectedOptions: SelectedCartOption[]
  ): string {
    const optionsKey = [...selectedOptions]
      .sort((firstOption, secondOption) =>
        firstOption.optionId.localeCompare(
          secondOption.optionId
        )
      )
      .map(
        option =>
          `${option.optionId}:${option.valueId}`
      )
      .join('|');

    return optionsKey
      ? `product-${productId}_${optionsKey}`
      : `product-${productId}`;
  }

  private loadInitialCart(): CartItem[] {
    if (!isPlatformBrowser(this.platformId)) {
      return [];
    }

    try {
      const storedCart = localStorage.getItem(
        this.storageKey
      );

      if (!storedCart) {
        return [];
      }

      const parsedCart: unknown = JSON.parse(storedCart);

      if (!Array.isArray(parsedCart)) {
        return [];
      }

      return parsedCart
        .filter(this.isStoredCartItem)
        .map(item => this.normalizeCartItem(item));
    } catch {
      return [];
    }
  }

  private isStoredCartItem(
    value: unknown
  ): value is Partial<CartItem> & {
    product: Product;
    quantity: number;
  } {
    if (!value || typeof value !== 'object') {
      return false;
    }

    const item = value as Partial<CartItem>;

    return (
      !!item.product &&
      typeof item.product === 'object' &&
      typeof item.product.id === 'number' &&
      typeof item.quantity === 'number' &&
      item.quantity > 0
    );
  }

  private normalizeCartItem(
    item: Partial<CartItem> & {
      product: Product;
      quantity: number;
    }
  ): CartItem {
    const selectedOptions = Array.isArray(
      item.selectedOptions
    )
      ? item.selectedOptions
      : [];

    const finalPrice =
      typeof item.finalPrice === 'number' &&
      item.finalPrice > 0
        ? item.finalPrice
        : item.product.discountPrice ??
          item.product.price;

    return {
      cartItemId:
        item.cartItemId ??
        this.createCartItemId(
          item.product.id,
          selectedOptions
        ),
      product: item.product,
      quantity: item.quantity,
      selectedOptions,
      finalPrice
    };
  }
}