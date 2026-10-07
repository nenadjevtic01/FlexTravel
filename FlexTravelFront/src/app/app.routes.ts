import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    title: 'Destinations · Flex Travel',
    loadComponent: () => import('./pages/destinations/destinations').then((m) => m.Destinations),
  },
  {
    path: 'destinations/:id',
    title: 'Destination · Flex Travel',
    loadComponent: () =>
      import('./pages/destination-detail/destination-detail').then((m) => m.DestinationDetailPage),
  },
  {
    path: 'trips',
    title: 'Trips · Flex Travel',
    loadComponent: () => import('./pages/trips/trips').then((m) => m.Trips),
  },
  {
    path: 'trips/:id',
    title: 'Trip · Flex Travel',
    loadComponent: () => import('./pages/trip-detail/trip-detail').then((m) => m.TripDetailPage),
  },
  {
    path: 'compare',
    title: 'Compare trips · Flex Travel',
    loadComponent: () => import('./pages/compare/compare').then((m) => m.Compare),
  },
  {
    path: 'agencies/:id',
    title: 'Agency · Flex Travel',
    loadComponent: () => import('./pages/agency-page/agency-page').then((m) => m.AgencyPageView),
  },
  {
    path: 'my-trips',
    title: 'My trips · Flex Travel',
    loadComponent: () => import('./pages/my-trips/my-trips').then((m) => m.MyTrips),
  },
  {
    path: 'profile',
    title: 'My profile · Flex Travel',
    loadComponent: () => import('./pages/profile/profile').then((m) => m.ProfilePage),
  },
  {
    path: 'agency/trips/:id/passengers',
    title: 'Passengers · Flex Travel',
    loadComponent: () => import('./pages/agency/passengers').then((m) => m.Passengers),
  },
  {
    path: 'favorites',
    title: 'Favorites · Flex Travel',
    loadComponent: () => import('./pages/favorites/favorites').then((m) => m.Favorites),
  },
  {
    path: 'agency',
    title: 'Agency panel · Flex Travel',
    loadComponent: () => import('./pages/agency/agency').then((m) => m.Agency),
  },
  {
    path: 'agency/trips/new',
    title: 'New trip · Flex Travel',
    loadComponent: () => import('./pages/agency/trip-edit').then((m) => m.TripEdit),
  },
  {
    path: 'agency/trips/:id/edit',
    title: 'Edit trip · Flex Travel',
    loadComponent: () => import('./pages/agency/trip-edit').then((m) => m.TripEdit),
  },
  {
    path: 'reservations/:id',
    title: 'Reservation · Flex Travel',
    loadComponent: () =>
      import('./pages/reservation-detail/reservation-detail').then((m) => m.ReservationDetailPage),
  },
  {
    path: 'admin',
    title: 'Admin panel · Flex Travel',
    loadComponent: () => import('./pages/admin/admin').then((m) => m.Admin),
  },
  {
    path: 'admin/destinations/:id',
    title: 'Edit destination · Flex Travel',
    loadComponent: () =>
      import('./pages/admin/destination-editor').then((m) => m.DestinationEditor),
  },
  {
    path: 'register',
    title: 'Create account · Flex Travel',
    loadComponent: () => import('./pages/register/register').then((m) => m.Register),
  },
  {
    path: 'countries',
    title: 'Countries · Flex Travel',
    loadComponent: () => import('./pages/countries/countries').then((m) => m.Countries),
  },
  {
    path: 'activity',
    title: 'Activity · Flex Travel',
    loadComponent: () => import('./pages/activity/activity').then((m) => m.Activity),
  },
  {
    path: 'login',
    title: 'Sign in · Flex Travel',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  { path: '**', redirectTo: '' },
];
