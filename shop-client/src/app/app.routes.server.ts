import { RenderMode, ServerRoute } from '@angular/ssr';

import { MOCK_CATEGORIES } from './core/mock-data/categories.mock';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'products/:id',
    renderMode: RenderMode.Server
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