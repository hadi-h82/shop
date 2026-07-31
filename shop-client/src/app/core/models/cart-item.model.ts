import { Product } from './product.model';

export interface SelectedCartOption {
  optionId: string;
  optionTitle: string;
  valueId: string;
  valueTitle: string;
  priceModifier: number;
}

export interface CartItem {
  /**
   * شناسه یکتای ترکیب محصول و گزینه‌های انتخاب‌شده
   */
  cartItemId: string;

  product: Product;
  quantity: number;

  /**
   * گزینه‌هایی مانند رنگ، اندازه و جنس
   */
  selectedOptions: SelectedCartOption[];

  /**
   * قیمت یک واحد محصول بعد از محاسبه گزینه‌ها
   */
  finalPrice: number;
}