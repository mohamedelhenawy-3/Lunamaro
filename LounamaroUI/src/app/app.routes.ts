import { Routes } from '@angular/router';
import { customerGuard } from './Components/Auth/customer.guard';
import { adminGuard } from './Components/Auth/admin.guard';
import { pendingorderguardGuard } from './Components/Guards/pendingorderguard.guard';
import { authGuard } from './Components/Auth/AuthGurad/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'Home', pathMatch: 'full' },

  {
    path: 'Home',
    loadComponent: () => import('./Components/home/home.component')
      .then(m => m.HomeComponent)
  },

  {
    path: 'login',
    loadComponent: () => import('./Components/Auth/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./Components/Auth/register/register.component')
      .then(m => m.RegisterComponent)
  },

  {
    path: 'menu/item/:id',
    loadComponent: () => import('./Components/item-detail/item-detail.component')
      .then(m => m.ItemDetailComponent)
  },
  {
    path: 'menu',
    loadComponent: () => import('./Components/menu/menu.component')
      .then(m => m.MenuComponent)
  },

  {
    path: 'usercartitempervew',
    loadComponent: () => import('./Components/user-cart-items/user-cart-items.component')
      .then(m => m.UserCartItemsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'orderpervew',
    loadComponent: () => import('./Components/order/order.component')
      .then(m => m.OrderComponent),
    canDeactivate: [pendingorderguardGuard],
    canActivate: [authGuard]
  },
  {
    path: 'ordershistory',
    loadComponent: () => import('./Components/userorderhistory/userorderhistory.component')
      .then(m => m.UserorderhistoryComponent),
    canActivate: [customerGuard]
  },
  {
    path: 'details/:id',
    loadComponent: () => import('./Components/orderdetails/orderdetails.component')
      .then(m => m.OrderdetailsComponent)
  },
  {
    path: 'payment-success/:sessionId',
    loadComponent: () => import('./Components/paymentsuccess/paymentsuccess.component')
      .then(m => m.PaymentsuccessComponent)
  },

  {
    path: 'Table',
    loadComponent: () => import('./Components/table/table.component')
      .then(m => m.TableComponent)
  },
  {
    path: 'reservation',
    loadComponent: () => import('./Components/reservation/reservation.component')
      .then(m => m.ReservationComponent)
  },
  {
    path: 'MyReservations',
    loadComponent: () => import('./Components/user-rescervations/user-rescervations.component')
      .then(m => m.UserRescervationsComponent),
    canActivate: [customerGuard]
  },

  {
    path: 'Reviews',
    loadComponent: () => import('./Components/review/review.component')
      .then(m => m.ReviewComponent)
  },

  {
    path: 'Admin',
    loadChildren: () => import('./Components/Admin/admin.routes')
      .then(m => m.adminRoutes),
    canActivate: [adminGuard]
  },

  { path: '**', redirectTo: 'Home' }
];