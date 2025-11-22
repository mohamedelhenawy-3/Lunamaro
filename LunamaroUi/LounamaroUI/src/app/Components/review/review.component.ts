import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { CreateReview } from '../../Models/Review/CreateReview';
import { ReviewsService } from '../../Service/Reviews/reviews.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './review.component.html',
  styleUrl: './review.component.css'
})
export class ReviewComponent implements OnInit {

  reviewsData?: ReviewResponse;

  newReview: CreateReview = {
    Rating: 0,
    Content: ''
  };

  constructor(private reviewService: ReviewsService) {}

  ngOnInit(): void {
    this.loadReviews();
  }

  loadReviews() {
    this.reviewService.getAllReviews().subscribe(
      res => this.reviewsData = res
    );
  }

  setRating(value: number) {
    this.newReview.Rating = value;
  }

  submitReview() {
    if (this.newReview.Rating < 1) return alert("Please add a rating");
    if (!this.newReview.Content.trim()) return alert("Please enter content");

    this.reviewService.CreateReview(this.newReview).subscribe(() => {
      alert("Review added successfully!");
      this.newReview = { Rating: 0, Content: '' };
      this.loadReviews();
    });
  }
}
