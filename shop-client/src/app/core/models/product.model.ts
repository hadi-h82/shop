export interface Product {
  id: number;
  title: string;
  slug: string;
  price: number;
  discountPrice?: number;
  imageUrl: string;
  categoryName: string;
  isAvailable: boolean;
}