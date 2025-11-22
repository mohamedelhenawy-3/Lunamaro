import { Component, OnInit } from '@angular/core';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { RouterLink } from '@angular/router';
import { ExploreItem } from '../../Models/item/exploreItem';
import { ItemService } from '../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { ReviewsService } from '../../Service/Reviews/reviews.service';

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


  constructor(private imgservice: ImageShareService, private itemService: ItemService,private reviewservice:ReviewsService) {}

  ngOnInit(): void {
    this.imgservice.updateImage(this.headerImage);

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
  }


}
