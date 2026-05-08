import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AddCategoryComponent } from './add-category/add-category.component';
import { CategoryComponent } from './category/category.component';
import { ItemComponent } from './item/item.component';
import { AddItemComponent } from './add-item/add-item.component';
import { UpdateItemComponent } from './updateitems/updateitems.component';
import { ControlledRecervationsComponent } from './ControlledRecervations/controlled-recervations/controlled-recervations.component';
import { adminGuard } from '../Auth/admin.guard';
import { ControlledtableComponent } from './controlledtable/controlledtable.component';
import { OffersComponent } from './offers/offers.component';
import { WeaklydealeditComponent } from './weaklydealedit/weaklydealedit.component';
import { EditdiscounttiersComponent } from './editdiscounttiers/editdiscounttiers.component';
import { AddweeklydealComponent } from './addweeklydeal/addweeklydeal.component';
import { CreatediscounttiersComponent } from './creatediscounttiers/creatediscounttiers.component';
import { AddonrewardComponent } from './addonreward/addonreward.component';
import { AddonrewardeditComponent } from './addonrewardedit/addonrewardedit.component';
import { UpdatedTableComponent } from './updated-table/updated-table.component';
import { ControlleddetailsComponent } from './controlleddetails/controlleddetails.component';
import { NewtableComponent } from './newtable/newtable.component';
import { ControlledOrderHistoryComponent } from './controlled-order-history/controlled-order-history.component';

export const adminRoutes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'AddCategory', component: AddCategoryComponent,canActivate:[adminGuard] },
  { path: 'category', component: CategoryComponent ,canActivate:[adminGuard]},
  { path: 'item', component: ItemComponent ,canActivate:[adminGuard]},
  { path: 'additem', component: AddItemComponent ,canActivate:[adminGuard]},
  { path: 'update-item/:id', component: UpdateItemComponent,canActivate:[adminGuard]  },
  {path:'details/:id',component:UpdatedTableComponent,canActivate:[adminGuard]},
  {path:'ordershistory',component:ControlledOrderHistoryComponent,canActivate:[adminGuard]},
  { path: 'order/details/:id', component: ControlleddetailsComponent, canActivate: [adminGuard] },
  {path:'AddNewTable',component:NewtableComponent,canActivate:[adminGuard]},

  {path:'reservation',component:ControlledRecervationsComponent,canActivate:[adminGuard]},
  {path:'table',component:ControlledtableComponent,canActivate:[adminGuard]},
  {path:'offers',component:OffersComponent,canActivate:[adminGuard]},
  {path:'offers/weekly-deal/:id',component:WeaklydealeditComponent,canActivate:[adminGuard]},
  {path:'offers/discount-tier/:id',component:EditdiscounttiersComponent,canActivate:[adminGuard]},
  
  {path:'createweeklydeals',component:AddweeklydealComponent,canActivate:[adminGuard]},
  {path:'creatediscounttiers',component:CreatediscounttiersComponent,canActivate:[adminGuard]},
  {path:'addonreward',component:AddonrewardComponent,canActivate:[adminGuard]},
  {path:'offers/addonrewardedit/:id',component:AddonrewardeditComponent,canActivate:[adminGuard]},
];