import { MOCK_CATEGORIES } from './core/mock-data/categories.mock';
import { serverRoutes } from './app.routes.server';

describe('serverRoutes', () => {
  it('prerenders every active category using its existing slug', async () => {
    const categoryRoute = serverRoutes.find(
      route => route.path === 'categories/:slug'
    );

    expect(categoryRoute).toBeTruthy();
    expect('getPrerenderParams' in categoryRoute!).toBe(true);

    if (!categoryRoute || !('getPrerenderParams' in categoryRoute)) {
      throw new Error('Category prerender route is not configured.');
    }

    const params = await categoryRoute.getPrerenderParams();
    const expectedParams = MOCK_CATEGORIES
      .filter(category => category.isActive)
      .map(category => ({ slug: category.slug }));

    expect(params).toEqual(expectedParams);
  });
});
