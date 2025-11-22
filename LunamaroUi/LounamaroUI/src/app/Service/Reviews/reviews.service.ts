import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CreateReview } from '../../Models/Review/CreateReview';

@Injectable({
  providedIn: 'root'
})
export class ReviewsService {
 constructor(private _HttpClient:HttpClient) { }




   getAllReviews(): Observable<ReviewResponse> {
    return this._HttpClient.get<ReviewResponse>(`${environment.baseurl}/Review`)
  }


getLatestReviews() {
  return this._HttpClient.get<ReviewResponse>(`${environment.baseurl}/Review/latest`);
}

  
 CreateReview(review: CreateReview) {
  return this._HttpClient.post<any>(`${environment.baseurl}/Review`, review);
}

}
