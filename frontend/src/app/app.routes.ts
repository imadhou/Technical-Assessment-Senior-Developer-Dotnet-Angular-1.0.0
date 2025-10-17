import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
export const routes: Routes = [
  { path: '', redirectTo: '/expenses', pathMatch: 'full' },
  {
    path: 'profile',
    loadComponent: () => import('./components/user/profile/profile.component').then(m => m.ProfileComponent),
    title: 'Profile - Expense Manager',
    canActivate: [authGuard]
  },
  {
    path: 'auth',
    loadChildren: () => import('./components/user/user.routes').then(m => m.userRoutes)
  },
  {
    path: 'expenses',
    loadComponent: () => import('./components/expense/expense-list/expense-list.component').then(m => m.ExpenseListComponent),
    title: 'Expense list',
    canActivate: [authGuard]
  },
  // Fallback
  { path: '**', redirectTo: '/expenses' }
];
