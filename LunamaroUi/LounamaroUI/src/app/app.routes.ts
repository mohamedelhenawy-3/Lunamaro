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

export const routes: Routes = [
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
   { path: 'home', component: HomeComponent },
   {path:'Admin/AddCategory',component:AddCategoryComponent,canActivate:[adminGuard]},
  {path:'Admin/category',component:CategoryComponent,canActivate:[adminGuard]},
   {path:'Admin/item',component:ItemComponent,canActivate:[adminGuard]},
      {path:'Admin/additem',component:AddItemComponent,canActivate:[adminGuard]},
       {path:'menu',component:MenuComponent},

  // Fallback
  { path: '**', redirectTo: '' }
];
