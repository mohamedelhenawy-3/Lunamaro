import { Routes } from '@angular/router';
import { LoginComponent } from './Components/Auth/login/login.component';
import { RegisterComponent } from './Components/Auth/register/register.component';
import { HomeComponent } from './Components/home/home.component';
import { MenuComponent } from './Components/menu/menu.component';
import { UserCartItemsComponent } from './Components/user-cart-items/user-cart-items.component';
import { ReservationComponent } from './Components/reservation/reservation.component';
import { TableComponent } from './Components/table/table.component';
import { UserRescervationsComponent } from './Components/user-rescervations/user-rescervations.component';
import { OrderComponent } from './Components/order/order.component';
import { PaymentsuccessComponent } from './Components/paymentsuccess/paymentsuccess.component';
import { UserorderhistoryComponent } from './Components/userorderhistory/userorderhistory.component';
import { OrderdetailsComponent } from './Components/orderdetails/orderdetails.component';
import { customerGuard } from './Components/Auth/customer.guard';
import { ReviewComponent } from './Components/review/review.component';
import { adminGuard } from './Components/Auth/admin.guard';
import { pendingorderguardGuard } from './Components/Guards/pendingorderguard.guard';
import { authGuard } from './Components/Auth/AuthGurad/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'Home', pathMatch: 'full' },
  { path: 'Home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'menu', loadComponent: () => import('./Components/menu/menu.component').then(m => m.MenuComponent) },
  { path: 'orderpervew', component: OrderComponent, canDeactivate: [pendingorderguardGuard], canActivate: [authGuard] },
  { path: 'usercartitempervew', component: UserCartItemsComponent, canActivate: [authGuard] },
  { path: 'Table', component: TableComponent },
  { path: 'ordershistory', component: UserorderhistoryComponent, canActivate: [customerGuard] },
  { path: 'details/:id', component: OrderdetailsComponent },
  { path: 'reservation', loadComponent: () => import('./Components/reservation/reservation.component').then(m => m.ReservationComponent) },
  { path: 'MyReservations', component: UserRescervationsComponent, canActivate: [customerGuard] },
  { path: 'payment-success/:sessionId', component: PaymentsuccessComponent },
  { path: 'Reviews', component: ReviewComponent },

  // ✅ ONE lazy-loaded Admin block — no duplicate routes above
  {
    path: 'Admin',
    loadChildren: () =>
      import('./Components/Admin/admin.routes').then(m => m.adminRoutes),
    canActivate: [adminGuard]
  },

  { path: '**', redirectTo: 'Home' }
];