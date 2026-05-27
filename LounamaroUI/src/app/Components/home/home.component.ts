import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { Router, RouterLink } from '@angular/router';
import { ExploreItem } from '../../Models/item/exploreItem';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { specialItem } from '../../Models/item/specialitems';
import { AddToCart } from '../../Models/add-to-cart';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { environment } from 'src/environments/environment.prod';
import { AuthService } from 'src/app/Service/auth.service';
import { catchError, of } from 'rxjs';
import { HomeService } from 'src/app/Service/HomeService/home.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {

  imageBaseUrl = environment.imageUrl;
  headerImage  = '/assets/Intro/Item.jpg';

  newestItems:     ExploreItem[] = [];
  bestSellerItems: any[]         = [];
  reviewsData?:    ReviewResponse;
  specialItems:    specialItem[] = [];
  weeklyDeals:     any[]         = [];
  discountTiers:   any[]         = [];
  rewards:         any[]         = [];
  isOnline = navigator.onLine;

  readonly rating       = +(4.6 + Math.random() * 0.3).toFixed(1);
  readonly reviewsCount = 800 + Math.floor(Math.random() * 600);

  constructor(
    private homeService:    HomeService,
    private cartsrviceapi:  UsercartService,
    private imgservice:     ImageShareService,
    private authService:    AuthService,
    private router:         Router,
    private cdr:            ChangeDetectorRef
  ) {}
ngOnInit(): void {
  this.imgservice.updateImage(this.headerImage);

  window.addEventListener('online',  () => { this.isOnline = true;  this.cdr.markForCheck(); });
  window.addEventListener('offline', () => { this.isOnline = false; this.cdr.markForCheck(); });

  this.homeService.getHomeData().pipe(
    catchError(() => {
      // ✅ Network failed AND no cache — set offline flag
      this.isOnline = false;
      this.cdr.markForCheck();
      return of(null);
    })
  ).subscribe(res => {
    // ✅ Handle offline response from sw-custom.js
    if (res?.offline === true || !res?.data) {
      this.isOnline = false;
      this.cdr.markForCheck();
      return;
    }

    this.isOnline = true;
    const data = res.data;

    this.bestSellerItems = (data.popular      || []).filter((i: any) => i.imageUrl?.startsWith('https://'));
    this.weeklyDeals     = (data.weeklyDeals   || []).filter((d: any) => d?.product?.imageUrl?.startsWith('https://'));
    this.discountTiers   = data.discountTiers  || [];
    this.rewards         = data.addOnRewards   || [];
    this.cdr.markForCheck();

    const scheduleSecondary = typeof requestIdleCallback !== 'undefined'
      ? (cb: () => void) => requestIdleCallback(cb, { timeout: 2000 })
      : (cb: () => void) => setTimeout(cb, 200);

    scheduleSecondary(() => {
      this.specialItems = (data.specialItems || []).filter((i: any) => i.imageUrl?.startsWith('https://'));
      this.reviewsData  = data.latestReviews;
      this.newestItems  = (data.menuPreview  || []).filter((i: any) => i.imageUrl?.startsWith('https://'));
      this.cdr.markForCheck();
    });
  });
}

  addtocart(itemid: number): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    const dto: AddToCart = { itemId: itemid, quantity: 1 };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
      this.cartsrviceapi.fetchCartCount();
    });
  }

  trackById(_index: number, item: any): number {
    return item.id;
  }

  onImageLoad(event: Event): void {
    (event.target as HTMLImageElement).classList.add('loaded');
  }

  getInitials(name: string): string {
    if (!name) return '?';
    const parts = name.trim().split(' ');
    return parts.length === 1
      ? parts[0][0].toUpperCase()
      : (parts[0][0] + parts[1][0]).toUpperCase();
  }

  getAvatarColor(name: string): string {
    const colors = [
      '#F44336', '#E91E63', '#9C27B0',
      '#3F51B5', '#2196F3', '#4CAF50',
      '#FF9800', '#795548'
    ];
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    return colors[Math.abs(hash) % colors.length];
  }
}