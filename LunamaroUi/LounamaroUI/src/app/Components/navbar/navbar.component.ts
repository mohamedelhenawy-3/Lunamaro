import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { CommonModule } from '@angular/common';
import { AuthService } from '../../Service/auth.service';
import { CategoryService } from '../../Service/Category/category.service';
import { UsercartService } from '../../Service/UserCart/usercart.service';

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
cartCount: number = 0;

  constructor(private auth: AuthService  ,private router:Router,private cartService:UsercartService) {}

ngOnInit(): void {
  this.auth.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.auth.getUserRole(); // 'Admin' or 'User'

    if (isLogged) {
      this.cartService.loadcount$.subscribe(count => {
        this.cartCount = count;
        console.log(count);
      });

      // Fetch initial count once (optional if already triggered from MenuComponent)
      this.cartService.fetchCartCount(); // ← Make sure this works without userId
    }
  });
}




  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
