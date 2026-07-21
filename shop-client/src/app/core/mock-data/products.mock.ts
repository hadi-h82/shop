import { Product } from '../models/product.model';

export const MOCK_PRODUCTS: Product[] = [
  {
    id: 1,
    categoryId: 1,
    title: 'سینی کیک MDF یک‌رو با ضخامت ۳ میلی‌متر',
    slug: 'single-sided-mdf-cake-board-3mm',
    price: 0,
    imageUrl: '/images/home/mdf-cake-board-one-side.webp',
    categoryName: 'سینی کیک',
    isAvailable: true,
    options: []
  },
  {
    id: 2,
    categoryId: 1,
    title: 'سینی کیک MDF دورو با ضخامت ۳ میلی‌متر',
    slug: 'double-sided-mdf-cake-board-3mm',
    price: 0,
    imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
    categoryName: 'سینی کیک',
    isAvailable: true,
    options: []
  },
  {
    id: 3,
    categoryId: 1,
    title: 'سینی کیک MDF دورو با ضخامت ۴ میلی‌متر',
    slug: 'double-sided-mdf-cake-board-4mm',
    price: 0,
    imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
    categoryName: 'سینی کیک',
    isAvailable: true,
    options: []
  },
  {
    id: 4,
    categoryId: 1,
    title: 'سینی کیک مقوایی',
    slug: 'cardboard-cake-board',
    price: 0,
    imageUrl: '/images/home/cardboard-cake-board.webp',
    categoryName: 'سینی کیک',
    isAvailable: true,
    options: []
  },
  {
    id: 5,
    categoryId: 1,
    title: 'زیردسری پلاستیکی کیک',
    slug: 'plastic-cake-board',
    price: 0,
    imageUrl: '/images/home/plastic-cake-board.webp',
    categoryName: 'سینی کیک',
    isAvailable: true,
    options: []
  }
];