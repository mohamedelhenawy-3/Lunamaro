import { Routes } from '@angular/router';
import { LoginComponent } from './Components/Auth/login/login.component';
import { RegisterComponent } from './Components/Auth/register/register.component';
import { HomeComponent } from './Components/home/home.component';
import { AddCategoryComponent } from './Components/Admin/add-category/add-category.component';
import { adminGuard } from './Components/Auth/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
   { path: 'home', component: HomeComponent },
   {path:'Admin/AddCategory',component:AddCategoryComponent,canActivate:[adminGuard]},
  // Fallback
  { path: '**', redirectTo: '' }
];
