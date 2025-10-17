import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: 'sign-in',
    loadComponent: () => import('./sign-in/sign-in.component').then(m => m.SignInComponent),
    title: 'Sign In - Expense Manager'
  },
  {
    path: 'sign-up',
    loadComponent: () => import('./sign-up/sign-up.component').then(m => m.SignUpComponent),
    title: 'Sign Up - Expense Manager'
  }
];
