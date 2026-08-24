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
            .then(m => m.AdminDashboard)
      }
    ]
  }
];