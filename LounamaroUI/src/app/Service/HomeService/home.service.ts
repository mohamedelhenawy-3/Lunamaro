import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, shareReplay, catchError, of } from 'rxjs';
import { OfflineService } from '../OfflineSerivce/offline-service.service';


@Injectable({ providedIn: 'root' })
export class HomeService {
  private cache$:   Observable<any> | null = null;
  private cacheTime = 0;
  private readonly TTL    = 60_000;
  private readonly apiUrl = 'https://lunamaro.runasp.net/api/Home/home-data';
  private readonly STORAGE_KEY = 'lunamaro_home_cache';

  constructor(
    private http: HttpClient,
    private offlineService: OfflineService
  ) {}

  getHomeData(): Observable<any> {
    // Offline — return localStorage cache immediately
    if (!this.offlineService.isOnline) {
      return of(this.getLocalCache());
    }

    const expired = Date.now() - this.cacheTime > this.TTL;
    if (!this.cache$ || expired) {
      this.cacheTime = Date.now();
      this.cache$ = this.http.get(this.apiUrl).pipe(
        shareReplay(1),
        catchError(() => of(this.getLocalCache()))
      );

      // Save fresh data to localStorage for offline use
      this.cache$.subscribe(data => {
        if (data) localStorage.setItem(this.STORAGE_KEY, JSON.stringify(data));
      });
    }

    return this.cache$;
  }

  private getLocalCache(): any {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }

  clearCache(): void {
    this.cache$    = null;
    this.cacheTime = 0;
  }
}