import { ProductOption } from './product-option.model';
export interface Product {
  id: number;
  categoryId?: number;
  title: string;
  slug: string;
  price: number;
  discountPrice?: number;
  imageUrl: string;
  categoryName: string;
  isAvailable: boolean;
  options?: ProductOption[];
}