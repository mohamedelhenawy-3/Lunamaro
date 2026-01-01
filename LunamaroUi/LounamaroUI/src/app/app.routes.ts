import { Routes } from '@angular/router';
import { LoginComponent } from './Components/Auth/login/login.component';
import { RegisterComponent } from './Components/Auth/register/register.component';
import { HomeComponent } from './Components/home/home.component';
import { AddCategoryComponent } from './Components/Admin/add-category/add-category.component';
import { adminGuard } from './Components/Auth/admin.guard';
import { CategoryComponent } from './Components/Admin/category/category.component';
import { ItemComponent } from './Components/Admin/item/item.component';
import { AddItemComponent } from './Components/Admin/add-item/add-item.component';
import { MenuComponent } from './Components/menu/menu.component';
// import { OrderperviewComponent } from './Components/orderperview/orderperview.component';
import { UserCartItemsComponent } from './Components/user-cart-items/user-cart-items.component';
import { ReservationComponent } from './Components/reservation/reservation.component';
import { ControlledRecervationsComponent } from './Components/Admin/ControlledRecervations/controlled-recervations/controlled-recervations.component';
import { ControlledtableComponent } from './Components/Admin/controlledtable/controlledtable.component';
import { TableComponent } from './Components/table/table.component';
import { UserRescervationsComponent } from './Components/user-rescervations/user-rescervations.component';
import { OrderComponent } from './Components/order/order.component';
import { PaymentsuccessComponent } from './Components/paymentsuccess/paymentsuccess.component';
import { UserorderhistoryComponent } from './Components/userorderhistory/userorderhistory.component';
import { OrderdetailsComponent } from './Components/orderdetails/orderdetails.component';
import { ControlledOrderHistoryComponent } from './Components/Admin/controlled-order-history/controlled-order-history.component';
import { customerGuard } from './Components/Auth/customer.guard';
import { ControlleddetailsComponent } from './Components/Admin/controlleddetails/controlleddetails.component';
import { UpdateItemComponent } from './Components/Admin/updateitems/updateitems.component';
import { DashboardComponent } from './Components/Admin/dashboard/dashboard.component';
import { UpdatedTableComponent } from './Components/Admin/updated-table/updated-table.component';
import { NewtableComponent } from './Components/Admin/newtable/newtable.component';
import { ReviewComponent } from './Components/review/review.component';
import { deactiveGuard } from './Components/Guards/DeactiveGuard/deactive.guard';
import { pendingorderguardGuard } from './Components/Guards/pendingorderguard.guard';
export const routes: Routes = [
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
    {path:'Admin/dashboard',component:DashboardComponent,canActivate:[adminGuard]},
   { path: 'Home', component: HomeComponent },
   {path:'Admin/AddCategory',component:AddCategoryComponent,canActivate:[adminGuard]},
  {path:'Admin/category',component:CategoryComponent,canActivate:[adminGuard]},
   {path:'Admin/additem',component:AddItemComponent,canActivate:[adminGuard],canDeactivate:[deactiveGuard]},
  {path:'Admin/item',component:ItemComponent,canActivate:[adminGuard]},
  {path:'Admin/update-item/:id',component:UpdateItemComponent,canActivate:[adminGuard]},
  {path:'menu',component:MenuComponent},
   {path:'orderpervew',component:OrderComponent ,canDeactivate:[pendingorderguardGuard]},
   {path:'usercartitempervew',component:UserCartItemsComponent},
  {path:'Table',component:TableComponent},
  {path:'ordershistory',component:UserorderhistoryComponent,canActivate:[customerGuard]},

  {path:'details/:id',component:OrderdetailsComponent},
  {path:'reservation',component:ReservationComponent},
 {path:'MyReservations',component:UserRescervationsComponent,canActivate:[customerGuard]},
  {path:'Admin/reservation',component:ControlledRecervationsComponent,canActivate:[adminGuard]},
  {path:'Admin/table',component:ControlledtableComponent,canActivate:[adminGuard]},

    {path:'Admin/details/:id',component:UpdatedTableComponent,canActivate:[adminGuard]},

{ path: 'payment-success/:sessionId', component:PaymentsuccessComponent },


  {path:'Admin/ordershistory',component:ControlledOrderHistoryComponent,canActivate:[adminGuard]},
{ path: 'Admin/order/details/:id', component: ControlleddetailsComponent, canActivate: [adminGuard] },
  {path:'Admin/AddNewTable',component:NewtableComponent,canActivate:[adminGuard]},



      {path:'Reviews',component:ReviewComponent},

  // Fallback
  { path: '**', redirectTo: '' }
];
