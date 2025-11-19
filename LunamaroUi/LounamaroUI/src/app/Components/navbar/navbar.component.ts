import { Component, HostListener } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { CommonModule } from '@angular/common';
import { AuthService } from '../../Service/auth.service';
import { CategoryService } from '../../Service/Category/category.service';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { ImageShareService } from '../../Service/ImageService/image-share.service';

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


//  Header
  imageFromHome: string = '';
  isScrolled = false;
 


  constructor(private auth: AuthService  ,private router:Router,private cartService:UsercartService ,private imageService: ImageShareService) {}

ngOnInit(): void {
this.imageService.currentimage.subscribe(image => {
      this.imageFromHome = image;
    });

  this.auth.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.auth.getUserRole(); // 'Admin' or 'User'

    if (isLogged) {
      this.cartService.loadcount$.subscribe(count => {
        this.cartCount = count;
        console.log(count);
              this.cartService.fetchCartCount(); // ← Make sure this works without userId

      });

      // Fetch initial count once (optional if already triggered from MenuComponent)
    }
  });
}




  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }


   @HostListener('window:scroll', [])
  onWindowScroll() {
    const navbar = document.querySelector('.navbar');
    if (navbar) {
      if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
      } else {
        navbar.classList.remove('scrolled');
      }
    }
  }


}