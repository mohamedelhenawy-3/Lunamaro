import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment.prod';

@Injectable({ providedIn: 'root' })
export class CacheWarmerService {
  private http = inject(HttpClient);
  private base = environment.baseurl;

  warmCache(): void {
    if (!navigator.onLine) return;

    const simpleUrls = [
      `${this.base}/Item/popular`,
      `${this.base}/Item/SpecialItems`,
      `${this.base}/Item/menu-preview`,
      `${this.base}/admin/offers/weekly-deals`,
      `${this.base}/admin/offers/discount-tiers`,
      `${this.base}/admin/offers/add-on-rewards`,
        `${this.base}/Category`,           // ✅ added

    ];

    simpleUrls.forEach(url => {
      this.http.get(url).pipe(catchError(() => of(null))).subscribe();
    });

    const menuParams = new HttpParams()
      .set('page', '1')
      .set('pageSize', '12');

    this.http.get(`${this.base}/Item/menu`, { params: menuParams })
      .pipe(catchError(() => of(null)))
      .subscribe();
  }
}