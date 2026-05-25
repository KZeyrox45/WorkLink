import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./components/login/login.component').then((c) => c.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./components/register/register.component').then(
        (c) => c.RegisterComponent
      ),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./components/dashboard/dashboard.component').then(
        (c) => c.DashboardComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./components/profile/profile.component').then(
        (c) => c.ProfileComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'jobs',
    loadComponent: () =>
      import('./components/job-list/job-list.component').then(
        (c) => c.JobListComponent
      ),
  },
  {
    path: 'jobs/new',
    loadComponent: () =>
      import('./components/job-form/job-form.component').then(
        (c) => c.JobFormComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'jobs/:id',
    loadComponent: () =>
      import('./components/job-detail/job-detail.component').then(
        (c) => c.JobDetailComponent
      ),
  },
  {
    path: 'jobs/:id/edit',
    loadComponent: () =>
      import('./components/job-form/job-form.component').then(
        (c) => c.JobFormComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'users/:id',
    loadComponent: () =>
      import('./components/public-profile/public-profile.component').then(
        (c) => c.PublicProfileComponent
      ),
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' },
];
