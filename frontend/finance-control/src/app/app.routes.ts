import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(m => m.Login)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register/register')
        .then(m => m.Register)
  },
  {
    path: '',
    loadComponent: () =>
      import('./shared/components/layout/layout')
        .then(m => m.Layout),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard')
            .then(m => m.Dashboard)
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/categories/categories/categories')
            .then(m => m.Categories)
      },
      {
        path: 'transactions',
        loadComponent: () =>
          import('./features/transactions/transactions/transactions')
            .then(m => m.Transactions)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];