import { Component, Inject, OnInit } from '@angular/core';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { Router, RouterLink } from '@angular/router';
import { ExploreItem } from '../../Models/item/exploreItem';
import { ItemService } from '../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { ReviewsService } from '../../Service/Reviews/reviews.service';
import { specialItem } from '../../Models/item/specialitems';
import { OffersservicesService } from '../../Service/Offers/offersservices.service';
import { AddToCart } from '../../Models/add-to-cart';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { environment } from 'src/environments/environment.prod';
import { AuthService } from 'src/app/Service/auth.service';
import { forkJoin } from 'rxjs';
import { NetworkStatusServiceService } from 'src/app/Service/network-status-service.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink,CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  imageBaseUrl=environment.imageUrl;
  headerImage = '/assets/Intro/Item.jpg';
  newestItems: ExploreItem[] = [];
  bestSellerItems: any[] = [];
  reviewsData?: ReviewResponse;
  specialItems: specialItem[] = []; // <-- NEW: Special Items

weeklyDeals: any[] = [];
discountTiers: any[] = [];
rewards: any[] = [];


  private network = Inject(NetworkStatusServiceService);
  isOnline$ = this.network.online$;


rating = 4.6 + Math.random() * 0.3;
reviewsCount = 800 + Math.floor(Math.random() * 600);
  constructor(private offerService:OffersservicesService,private cartsrviceapi:UsercartService
    ,private imgservice: ImageShareService, private itemService: ItemService,private reviewservice:ReviewsService,private authService:AuthService,private router:Router) {}

  ngOnInit(): void {
    this.imgservice.updateImage(this.headerImage);
  forkJoin({
    newest:   this.itemService.getNewestItems(),
    best:     this.itemService.getBestSelerItems(),
    special:  this.itemService.getSpecialItems(),
    reviews:  this.reviewservice.getLatestReviews(),
    deals:    this.offerService.getWeeklyDeals(),
    tiers:    this.offerService.getDiscountTiers(),
    rewards:  this.offerService.getAddOnRewards(),
  }).subscribe({
    next: (res) => {
      this.newestItems     = res.newest;
      this.bestSellerItems = res.best;
      this.specialItems    = res.special;
      this.reviewsData     = res.reviews;
      this.weeklyDeals     = res.deals?.data   || [];
      this.discountTiers   = res.tiers?.data   || [];
      this.rewards         = res.rewards?.data || [];
    },
    error: (err) => console.error('Home load error', err)
  });



  }



 addtocart(itemid:number){
    if (!this.authService.isLoggedIn()) {
    this.router.navigate(['/login']);
    return;
  }

    const dto: AddToCart = {    
     itemId: itemid,
      quantity: 1              
    };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
    this.cartsrviceapi.fetchCartCount(); 
  });
    }



trackById(index: number, item: any) {
  return item.id;
}

onImageLoad(event: any) {
  event.target.classList.add('loaded');
}
loadOffers() {

  this.offerService.getWeeklyDeals().subscribe(res => {
    this.weeklyDeals = res?.data || [];
  });

  this.offerService.getDiscountTiers().subscribe(res => {
    this.discountTiers = res?.data || [];
  });

  this.offerService.getAddOnRewards().subscribe(res => {
    this.rewards = res?.data || [];
  });

}
getInitials(name: string): string {
  if (!name) return '?';

  const parts = name.trim().split(' ');
  if (parts.length === 1) return parts[0][0].toUpperCase();

  return (parts[0][0] + parts[1][0]).toUpperCase();
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
addToCart(id:number){
alert("Added");
}
}
