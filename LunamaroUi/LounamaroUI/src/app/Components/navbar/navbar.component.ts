import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { CommonModule } from '@angular/common';
import { AuthService } from '../../Service/auth.service';

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

  constructor(private auth: AuthService  ,private router:Router) {}

ngOnInit(): void {
  this.auth.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.auth.getUserRole(); // 'Admin' or 'User'
  });
}


  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
