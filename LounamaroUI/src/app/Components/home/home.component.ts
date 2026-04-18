import { Component, OnInit } from '@angular/core';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { RouterLink } from '@angular/router';
import { ExploreItem } from '../../Models/item/exploreItem';
import { ItemService } from '../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { ReviewsService } from '../../Service/Reviews/reviews.service';
import { specialItem } from '../../Models/item/specialitems';
import { OffersservicesService } from '../../Service/Offers/offersservices.service';
import { AddToCart } from '../../Models/add-to-cart';
import { UsercartService } from '../../Service/UserCart/usercart.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink,CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  headerImage = '/assets/Intro/Item.jpg';
  newestItems: ExploreItem[] = [];
  bestSellerItems: ExploreItem[] = [];
  reviewsData?: ReviewResponse;
  specialItems: specialItem[] = []; // <-- NEW: Special Items

weeklyDeals: any[] = [];
discountTiers: any[] = [];
rewards: any[] = [];
  constructor(private offerService:OffersservicesService,private cartsrviceapi:UsercartService
    ,private imgservice: ImageShareService, private itemService: ItemService,private reviewservice:ReviewsService) {}

  ngOnInit(): void {
    this.imgservice.updateImage(this.headerImage);
  this.loadOffers();

    // Load newest menu items
    this.itemService.getNewestItems().subscribe(res => {
      this.newestItems = res;
      console.log(res);
    });

    // Load best seller items
    this.itemService.getBestSelerItems().subscribe(res => {
      this.bestSellerItems = res;
    });

     this.reviewservice.getLatestReviews().subscribe(
    res => this.reviewsData = res,
    err => console.error(err)
  );
     // Load special items
    this.itemService.getSpecialItems().subscribe({
      next: (res) => {
        this.specialItems = res
      console.log("res",res);},
      error: (err) => console.error('Failed to load special items', err)
    });

  }



 addtocart(itemid:number){
  
    const dto: AddToCart = {     // Use dynamic userId if available
     itemId: itemid,
      quantity: 1               // default quantity
    };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
    this.cartsrviceapi.fetchCartCount(); // No need for userId
  });
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
