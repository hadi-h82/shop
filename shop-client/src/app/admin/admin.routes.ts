import { Routes } from '@angular/router';

import { AdminLayout } from './layout/admin-layout/admin-layout';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayout,

    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/dashboard/dashboard')
            .then((m) => m.AdminDashboard),
      },

      {
        path: 'products',
        loadComponent: () =>
          import('./pages/products/product-list/product-list')
            .then((m) => m.AdminProductList),
      },

      {
        path: 'products/create',
        loadComponent: () =>
          import('./pages/products/product-create/product-create')
            .then((m) => m.AdminProductCreate),
      },
    ],
  },
];