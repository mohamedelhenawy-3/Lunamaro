import { Component, OnInit, OnDestroy, HostListener, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../Service/auth.service';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit, OnDestroy {
  isLoggedIn = false;
  userRole: string | null = null;
  cartCount = 0;
  isScrolled = false;
  menuOpen = false;
  imageFromHome = '';

  private subs = new Subscription();

  constructor(
    private cdr: ChangeDetectorRef ,
    public auth: AuthService,
    private router: Router,
    private cartService: UsercartService,
    private imageService: ImageShareService
  ) {}

  ngOnInit(): void {
    this.subs.add(
      this.imageService.currentimage.subscribe(image => {
        this.imageFromHome = image;
      })
    );

    this.subs.add(
      this.auth.isLoggedIn$.subscribe(isLogged => {
        this.isLoggedIn = isLogged;
        this.userRole = this.auth.getUserRole();
    if (isLogged) {
        this.userRole = this.auth.getUserRole(); // set role on login
        setTimeout(() => this.cartService.fetchCartCount(), 0);
      } else {
        this.userRole = null;  
        this.cartCount = 0;
      }

        this.cdr.detectChanges(); 

      })
    );

    this.subs.add(
      this.cartService.loadcount$.subscribe(count => {
        this.cartCount = count;
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe(); // prevent memory leaks
  }

  toggleMenu() { this.menuOpen = !this.menuOpen; }
  closeMenu()  { this.menuOpen = false; }

logout() {
  this.auth.logout();
  this.router.navigateByUrl('/login');
  this.cdr.detectChanges(); // ✅ add this
}

  @HostListener('window:scroll', [])
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50; // Angular handles class binding
  }
}