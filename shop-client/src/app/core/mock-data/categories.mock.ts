import { Category } from '../models/category.model';

export const MOCK_CATEGORIES: Category[] = [
  {
    id: 1,
    name: 'سینی کیک',
    slug: 'cake-trays',
    description: 'انواع سینی مناسب کیک و شیرینی',
    imageUrl: '/images/home/category-baking-tools.webp',
    displayOrder: 1,
    isActive: true
  },
  {
    id: 2,
    name: 'سینی فینگر فود',
    slug: 'finger-food-trays',
    description: 'سینی‌های مناسب سرو فینگر فود',
    imageUrl: '/images/home/category-decoration.webp',
    displayOrder: 2,
    isActive: true
  },
  {
    id: 3,
    name: 'پلکسی',
    slug: 'plexiglass',
    description: 'انواع محصولات پلکسی طرح‌دار و ساده',
    imageUrl: '/images/home/category-ingredients.webp',
    displayOrder: 3,
    isActive: true
  },
  {
    id: 4,
    name: 'باکس',
    slug: 'boxes',
    description: 'باکس‌های متنوع برای چیدمان و ارائه',
    imageUrl: '/images/home/category-colors.webp',
    displayOrder: 4,
    isActive: true
  },
  {
    id: 5,
    name: 'استنسیل',
    slug: 'stencils',
    description: 'طرح‌های متنوع استنسیل برای تزئین',
    imageUrl: '/images/home/category-decoration.webp',
    displayOrder: 5,
    isActive: true
  },
  {
    id: 6,
    name: 'جعبه',
    slug: 'packaging-boxes',
    description: 'انواع جعبه کیک، شیرینی و هدیه',
    imageUrl: '/images/home/category-packaging.webp',
    displayOrder: 6,
    isActive: true
  },
  {
    id: 7,
    name: 'ابزار و لوازم قنادی',
    slug: 'pastry-tools',
    description: 'ابزارهای کاربردی برای پخت و تزئین',
    imageUrl: '/images/home/category-equipment.webp',
    displayOrder: 7,
    isActive: true
  }
];