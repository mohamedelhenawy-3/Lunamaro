import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { CreateReview } from '../../Models/Review/CreateReview';
import { ReviewsService } from '../../Service/Reviews/reviews.service';
import { AuthService } from '../../Service/auth.service';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './review.component.html',
  styleUrl: './review.component.css'
})
export class ReviewComponent implements OnInit {

  reviewsData?: ReviewResponse;

  newReview: CreateReview = {
    Rating: 0,
    Content: ''
  };

  role: string | null = null;   // <<< IMPORTANT

  constructor(
    private reviewService: ReviewsService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getUserRole();  // <<< DETECT ROLE
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

  deleteReview(id: number) {
    if (!confirm("Are you sure you want to delete this review?")) return;

    this.reviewService.cancelReservation(id).subscribe(() => {
      alert("Review removed.");
      this.loadReviews();
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

}
