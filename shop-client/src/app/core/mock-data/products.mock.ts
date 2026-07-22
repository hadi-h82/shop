import { Product } from '../models/product.model';
const CAKE_TRAY_PRODUCTS: Product[] = [
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

const FINGER_FOOD_TRAY_PRODUCTS: Product[] = [
  {
    id: 201,
    categoryId: 2,
    title: 'سینی فینگر فود - مربع',
    slug: 'mdf-square-finger-food-tray',
    price: 0,
    imageUrl: '/images/home/mdf-square-finger-food-tray.webp',
    categoryName: 'سینی فینگر فود',
    isAvailable: true,
    options: []
  },
  {
    id: 202,
    categoryId: 2,
    title: 'سینی فینگر فود -  مستطیل',
    slug: 'mdf-rectangle-finger-food-tray',
    price: 0,
    imageUrl: '/images/home/mdf-rectangle-finger-food-tray.webp',
    categoryName: 'سینی فینگر فود',
    isAvailable: true,
    options: []
  },
  {
    id: 203,
    categoryId: 2,
    title: 'سینی فینگر فود - مدایره ای',
    slug: 'mdf-circle-finger-food-tray',
    price: 0,
    imageUrl: '/images/home/mdf-circle-finger-food-tray.webp',
    categoryName: 'سینی فینگر فود',
    isAvailable: true,
    options: []
  },
    {
    id: 204,
    categoryId: 2,
    title: 'سینی فینگر فود - دوبل',
    slug: 'finger-food-trays-double',
    price: 0,
    imageUrl: '/images/home/mdf-double-finger-food-tray.webp',
    categoryName: 'سینی فینگر فود',
    isAvailable: true,
    options: []
  },
  ];

const PLEXIGLASS_PRODUCTS: Product[] = [
  {
    id: 301,
    categoryId: 3,
    title: 'صفحه پلکسی ایرانی',
    slug: 'plexiglass-iranian-sheet',
    price: 0,
    imageUrl: '/images/home/plexiglass-iranian-sheet.webp',
    categoryName: 'پلکسی',
    isAvailable: true,
    options: []
  },
  // {
  //   id: 302,
  //   categoryId: 3,
  //   title: 'پلکسی - محصول نمونه ۲',
  //   slug: 'plexiglass-sample-2',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
  //   categoryName: 'پلکسی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 303,
  //   categoryId: 3,
  //   title: 'پلکسی - محصول نمونه ۳',
  //   slug: 'plexiglass-sample-3',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
  //   categoryName: 'پلکسی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 304,
  //   categoryId: 3,
  //   title: 'پلکسی - محصول نمونه ۴',
  //   slug: 'plexiglass-sample-4',
  //   price: 0,
  //   imageUrl: '/images/home/cardboard-cake-board.webp',
  //   categoryName: 'پلکسی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 305,
  //   categoryId: 3,
  //   title: 'پلکسی - محصول نمونه ۵',
  //   slug: 'plexiglass-sample-5',
  //   price: 0,
  //   imageUrl: '/images/home/plastic-cake-board.webp',
  //   categoryName: 'پلکسی',
  //   isAvailable: true,
  //   options: []
  // }
];

const BOX_PRODUCTS: Product[] = [
  // {
  //   id: 401,
  //   categoryId: 4,
  //   title: 'باکس - محصول نمونه ۱',
  //   slug: 'boxes-sample-1',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-one-side.webp',
  //   categoryName: 'باکس',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 402,
  //   categoryId: 4,
  //   title: 'باکس - محصول نمونه ۲',
  //   slug: 'boxes-sample-2',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
  //   categoryName: 'باکس',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 403,
  //   categoryId: 4,
  //   title: 'باکس - محصول نمونه ۳',
  //   slug: 'boxes-sample-3',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
  //   categoryName: 'باکس',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 404,
  //   categoryId: 4,
  //   title: 'باکس - محصول نمونه ۴',
  //   slug: 'boxes-sample-4',
  //   price: 0,
  //   imageUrl: '/images/home/cardboard-cake-board.webp',
  //   categoryName: 'باکس',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 405,
  //   categoryId: 4,
  //   title: 'باکس - محصول نمونه ۵',
  //   slug: 'boxes-sample-5',
  //   price: 0,
  //   imageUrl: '/images/home/plastic-cake-board.webp',
  //   categoryName: 'باکس',
  //   isAvailable: true,
  //   options: []
  // }
];

const STENCIL_PRODUCTS: Product[] = [
  // {
  //   id: 501,
  //   categoryId: 5,
  //   title: 'استنسیل - محصول نمونه ۱',
  //   slug: 'stencils-sample-1',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-one-side.webp',
  //   categoryName: 'استنسیل',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 502,
  //   categoryId: 5,
  //   title: 'استنسیل - محصول نمونه ۲',
  //   slug: 'stencils-sample-2',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
  //   categoryName: 'استنسیل',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 503,
  //   categoryId: 5,
  //   title: 'استنسیل - محصول نمونه ۳',
  //   slug: 'stencils-sample-3',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
  //   categoryName: 'استنسیل',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 504,
  //   categoryId: 5,
  //   title: 'استنسیل - محصول نمونه ۴',
  //   slug: 'stencils-sample-4',
  //   price: 0,
  //   imageUrl: '/images/home/cardboard-cake-board.webp',
  //   categoryName: 'استنسیل',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 505,
  //   categoryId: 5,
  //   title: 'استنسیل - محصول نمونه ۵',
  //   slug: 'stencils-sample-5',
  //   price: 0,
  //   imageUrl: '/images/home/plastic-cake-board.webp',
  //   categoryName: 'استنسیل',
  //   isAvailable: true,
  //   options: []
  // }
];

const PACKAGING_BOX_PRODUCTS: Product[] = [
  // {
  //   id: 601,
  //   categoryId: 6,
  //   title: 'جعبه - محصول نمونه ۱',
  //   slug: 'packaging-boxes-sample-1',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-one-side.webp',
  //   categoryName: 'جعبه',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 602,
  //   categoryId: 6,
  //   title: 'جعبه - محصول نمونه ۲',
  //   slug: 'packaging-boxes-sample-2',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
  //   categoryName: 'جعبه',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 603,
  //   categoryId: 6,
  //   title: 'جعبه - محصول نمونه ۳',
  //   slug: 'packaging-boxes-sample-3',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
  //   categoryName: 'جعبه',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 604,
  //   categoryId: 6,
  //   title: 'جعبه - محصول نمونه ۴',
  //   slug: 'packaging-boxes-sample-4',
  //   price: 0,
  //   imageUrl: '/images/home/cardboard-cake-board.webp',
  //   categoryName: 'جعبه',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 605,
  //   categoryId: 6,
  //   title: 'جعبه - محصول نمونه ۵',
  //   slug: 'packaging-boxes-sample-5',
  //   price: 0,
  //   imageUrl: '/images/home/plastic-cake-board.webp',
  //   categoryName: 'جعبه',
  //   isAvailable: true,
  //   options: []
  // }
];

const PASTRY_TOOL_PRODUCTS: Product[] = [
  // {
  //   id: 701,
  //   categoryId: 7,
  //   title: 'ابزار و لوازم قنادی - محصول نمونه ۱',
  //   slug: 'pastry-tools-sample-1',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-one-side.webp',
  //   categoryName: 'ابزار و لوازم قنادی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 702,
  //   categoryId: 7,
  //   title: 'ابزار و لوازم قنادی - محصول نمونه ۲',
  //   slug: 'pastry-tools-sample-2',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-3mm.webp',
  //   categoryName: 'ابزار و لوازم قنادی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 703,
  //   categoryId: 7,
  //   title: 'ابزار و لوازم قنادی - محصول نمونه ۳',
  //   slug: 'pastry-tools-sample-3',
  //   price: 0,
  //   imageUrl: '/images/home/mdf-cake-board-two-side-4mm.webp',
  //   categoryName: 'ابزار و لوازم قنادی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 704,
  //   categoryId: 7,
  //   title: 'ابزار و لوازم قنادی - محصول نمونه ۴',
  //   slug: 'pastry-tools-sample-4',
  //   price: 0,
  //   imageUrl: '/images/home/cardboard-cake-board.webp',
  //   categoryName: 'ابزار و لوازم قنادی',
  //   isAvailable: true,
  //   options: []
  // },
  // {
  //   id: 705,
  //   categoryId: 7,
  //   title: 'ابزار و لوازم قنادی - محصول نمونه ۵',
  //   slug: 'pastry-tools-sample-5',
  //   price: 0,
  //   imageUrl: '/images/home/plastic-cake-board.webp',
  //   categoryName: 'ابزار و لوازم قنادی',
  //   isAvailable: true,
  //   options: []
  // }
];

export const MOCK_PRODUCTS: Product[] = [
  ...CAKE_TRAY_PRODUCTS,
  ...FINGER_FOOD_TRAY_PRODUCTS,
  ...PLEXIGLASS_PRODUCTS,
  ...BOX_PRODUCTS,
  ...STENCIL_PRODUCTS,
  ...PACKAGING_BOX_PRODUCTS,
  ...PASTRY_TOOL_PRODUCTS
];
