import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReviewResponse } from '../../Models/Review/ReviewResponse';
import { Observable, of, catchError, tap } from 'rxjs';
import { environment } from '../../../environments/environment.prod';
import { CreateReview } from '../../Models/Review/CreateReview';
import { OfflineService } from '../OfflineSerivce/offline-service.service';

@Injectable({ providedIn: 'root' })
export class ReviewsService {
  private readonly REVIEWS_KEY        = 'lunamaro_reviews_cache';
  private readonly LATEST_REVIEWS_KEY = 'lunamaro_latest_reviews_cache';

  constructor(
    private _HttpClient: HttpClient,
    private offlineService: OfflineService
  ) {}

  getAllReviews(): Observable<ReviewResponse> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.REVIEWS_KEY));
    }
    return this._HttpClient.get<ReviewResponse>(
      `${environment.baseurl}/Review`
    ).pipe(
      tap(data => this.saveCache(this.REVIEWS_KEY, data)),
      catchError(() => of(this.getCache(this.REVIEWS_KEY)))
    );
  }

  getLatestReviews(): Observable<ReviewResponse> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.LATEST_REVIEWS_KEY));
    }
    return this._HttpClient.get<ReviewResponse>(
      `${environment.baseurl}/Review/latest`
    ).pipe(
      tap(data => this.saveCache(this.LATEST_REVIEWS_KEY, data)),
      catchError(() => of(this.getCache(this.LATEST_REVIEWS_KEY)))
    );
  }

  // ✅ Disabled offline — can't submit without internet
  CreateReview(review: CreateReview): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this._HttpClient.post<any>(`${environment.baseurl}/Review`, review);
  }

  cancelReservation(Id: number): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this._HttpClient.delete(`${environment.baseurl}/Review/admin/${Id}`);
  }

  private saveCache(key: string, data: any): void {
    try { localStorage.setItem(key, JSON.stringify(data)); } catch {}
  }

  private getCache(key: string): any {
    try {
      const stored = localStorage.getItem(key);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }
}