import { Product } from '../models/product.model';

export const MOCK_PRODUCTS: Product[] = [
  {
    id: 1,
    title: 'گوشی موبایل سامسونگ Galaxy S25',
    slug: 'samsung-galaxy-s25',
    price: 65000000,
    discountPrice: 61500000,
    imageUrl: '/images/products/samsung-galaxy-s25.png',
    categoryName: 'موبایل',
    isAvailable: true
  },
  {
    id: 2,
    title: 'لپ‌تاپ اپل MacBook Air M4',
    slug: 'apple-macbook-air-m4',
    price: 98000000,
    imageUrl: '/images/products/macbook-air-m4.jpg',
    categoryName: 'لپ‌تاپ',
    isAvailable: true
  },
  {
    id: 3,
    title: 'هدفون بی‌سیم Sony WH-1000XM5',
    slug: 'sony-wh-1000xm5',
    price: 24500000,
    discountPrice: 21900000,
    imageUrl: '/images/products/sony-wh-1000xm5.avif',
    categoryName: 'لوازم جانبی',
    isAvailable: true
  },
  {
    id: 4,
    title: 'ساعت هوشمند Apple Watch Series 10',
    slug: 'apple-watch-series-10',
    price: 32000000,
    imageUrl: '/images/products/apple-watch-series-10.jpg',
    categoryName: 'ساعت هوشمند',
    isAvailable: false
  }
];