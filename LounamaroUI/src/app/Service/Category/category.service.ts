import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, shareReplay, catchError, of, tap } from 'rxjs';
import { Category } from '../../Models/category';
import { environment } from '../../../environments/environment.prod';
import { OfflineService } from '../OfflineSerivce/offline-service.service';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly CACHE_KEY = 'lunamaro_categories';
  private categories$: Observable<Category[]> | null = null;

  constructor(
    private HttpClient: HttpClient,
    private offlineService: OfflineService
  ) {}

  getallCategories(): Observable<Category[]> {
    // Offline — return localStorage cache immediately
    if (!this.offlineService.isOnline) {
      return of(this.getCache() ?? []);
    }

    if (!this.categories$) {
      this.categories$ = this.HttpClient
        .get<Category[]>(`${environment.baseurl}/Category`)
        .pipe(
          tap(data  => this.saveCache(data)),
          shareReplay(1),
          catchError(() => of(this.getCache() ?? []))
        );
    }

    return this.categories$;
  }

  getCategoryById(id: number): Observable<Category> {
    if (!this.offlineService.isOnline) {
      const cached = this.getCache();
      const found  = cached?.find((c: Category) => c.id === id);
      return of(found ?? {} as Category);
    }
    return this.HttpClient
      .get<Category>(`${environment.baseurl}/category/${id}`)
      .pipe(catchError(() => of({} as Category)));
  }

  // ✅ Disabled offline
  createCategory(category: Partial<Category>): Observable<Category> {
    if (!this.offlineService.isOnline) return of({} as Category);
    return this.HttpClient.post<Category>(
      `${environment.baseurl}/Category/CreateCategory`, category
    );
  }

  // ✅ Disabled offline
  deleteCategory(id: number): Observable<void> {
    if (!this.offlineService.isOnline) return of(undefined);
    return this.HttpClient.delete<void>(`${environment.baseurl}/Category/${id}`);
  }

  private saveCache(data: Category[]): void {
    try { localStorage.setItem(this.CACHE_KEY, JSON.stringify(data)); } catch {}
  }

  private getCache(): Category[] | null {
    try {
      const stored = localStorage.getItem(this.CACHE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }
}