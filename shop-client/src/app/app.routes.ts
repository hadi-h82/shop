import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';
import { Home } from './pages/home/home';
import { Products } from './pages/products/products';
import { ProductDetail } from './pages/product-detail/product-detail';
import { Cart } from './pages/cart/cart';
import { Checkout } from './pages/checkout/checkout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      { path: '', component: Home },
      { path: 'products', component: Products },
      { path: 'products/:id', component: ProductDetail },
      { path: 'cart', component: Cart },
      { path: 'checkout', component: Checkout },
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/auth/login/login').then(m => m.Login)
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./pages/auth/register/register').then(m => m.Register)
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./pages/auth/forgot-password/forgot-password')
            .then(m => m.ForgotPassword)
      },
      {
        path: 'reset-password',
        loadComponent: () =>
          import('./pages/auth/reset-password/reset-password')
            .then(m => m.ResetPassword)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];