import { RenderMode, ServerRoute } from '@angular/ssr';

import { MOCK_CATEGORIES } from './core/mock-data/categories.mock';
import { MOCK_PRODUCTS } from './core/mock-data/products.mock';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'products/:id',
    renderMode: RenderMode.Prerender,
    async getPrerenderParams() {
      return MOCK_PRODUCTS.map(product => ({
         id: product.slug
      }));
    }
  },
  {
    path: 'categories/:slug',
    renderMode: RenderMode.Prerender,
    async getPrerenderParams() {
      return MOCK_CATEGORIES
        .filter(category => category.isActive)
        .map(category => ({
          slug: category.slug
        }));
    }
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];