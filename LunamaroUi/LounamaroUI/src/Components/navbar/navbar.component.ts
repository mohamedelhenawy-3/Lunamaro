import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../app/Service/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
 isLoggedIn: boolean = false;
  userRole: string | null = null;

  constructor(private authService: AuthService  ,private router:Router) {}

ngOnInit(): void {
  this.authService.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.authService.getUserRole(); // 'Admin' or 'User'
  });
}


  logout() {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
