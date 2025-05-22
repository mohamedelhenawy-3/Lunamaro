import { Routes } from '@angular/router';
import { LoginComponent } from '../Components/Auth/login/login.component';
import { RegisterComponent } from '../Components/Auth/register/register.component';
import { HomeComponent } from '../Components/home/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
   { path: 'home', component: HomeComponent },
  // Fallback
  { path: '**', redirectTo: '' }
];
