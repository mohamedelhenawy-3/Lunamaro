import { Review } from "./review";

export interface ReviewResponse {
  averageRating: number;
  totalReviews: number;
  reviews: Review[];
}