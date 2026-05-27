import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Item } from '../../Models/item';
import { environment } from '../../../environments/environment.prod';
import { Observable, shareReplay, catchError, of, tap } from 'rxjs';
import { ExploreItem } from '../../Models/item/exploreItem';
import { ReturnedItem } from '../../Models/item/returnedItem';
import { specialItem } from '../../Models/item/specialitems';
import { OfflineService } from '../OfflineSerivce/offline-service.service';
@Injectable({ providedIn: 'root' })
export class ItemService {

  // Cache keys
  private readonly KEYS = {
    bestSellers:  'lunamaro_best_sellers',
    newestItems:  'lunamaro_newest_items',
    specialItems: 'lunamaro_special_items',
    allItems:     'lunamaro_all_items',
    menu:         'lunamaro_menu',        // prefix — appended with page/cat
    item:         'lunamaro_item_',       // prefix — appended with id
  };

  // In-memory cache (session)
  private bestSellers$:  Observable<any[]>       | null = null;
  private newestItems$:  Observable<ExploreItem[]>| null = null;
  private specialItems$: Observable<specialItem[]>| null = null;
  private allItems$:     Observable<Item[]>       | null = null;

  constructor(
    private _HttpClient: HttpClient,
    private offlineService: OfflineService
  ) {}

  // ── Best Sellers ──────────────────────────────────────
  getBestSelerItems(): Observable<any[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.KEYS.bestSellers) ?? []);
    }
    if (!this.bestSellers$) {
      this.bestSellers$ = this._HttpClient
        .get<any[]>(`${environment.baseurl}/Item/popular`)
        .pipe(
          tap(data => this.saveCache(this.KEYS.bestSellers, data)),
          shareReplay(1),
          catchError(() => of(this.getCache(this.KEYS.bestSellers) ?? []))
        );
    }
    return this.bestSellers$;
  }

  // ── Newest Items ──────────────────────────────────────
  getNewestItems(): Observable<ExploreItem[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.KEYS.newestItems) ?? []);
    }
    if (!this.newestItems$) {
      this.newestItems$ = this._HttpClient
        .get<ExploreItem[]>(`${environment.baseurl}/Item/menu-preview`)
        .pipe(
          tap(data => this.saveCache(this.KEYS.newestItems, data)),
          shareReplay(1),
          catchError(() => of(this.getCache(this.KEYS.newestItems) ?? []))
        );
    }
    return this.newestItems$;
  }

  // ── Special Items ─────────────────────────────────────
  getSpecialItems(): Observable<specialItem[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.KEYS.specialItems) ?? []);
    }
    if (!this.specialItems$) {
      this.specialItems$ = this._HttpClient
        .get<specialItem[]>(`${environment.baseurl}/Item/SpecialItems`)
        .pipe(
          tap(data => this.saveCache(this.KEYS.specialItems, data)),
          shareReplay(1),
          catchError(() => of(this.getCache(this.KEYS.specialItems) ?? []))
        );
    }
    return this.specialItems$;
  }

  // ── All Items ─────────────────────────────────────────
  getallItems(): Observable<Item[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.KEYS.allItems) ?? []);
    }
    if (!this.allItems$) {
      this.allItems$ = this._HttpClient
        .get<Item[]>(`${environment.baseurl}/Item/AllNote`)
        .pipe(
          tap(data => this.saveCache(this.KEYS.allItems, data)),
          shareReplay(1),
          catchError(() => of(this.getCache(this.KEYS.allItems) ?? []))
        );
    }
    return this.allItems$;
  }

  // ── Paginated Menu — cached per page+category ─────────
  getItems(
    page: number = 1,
    pageSize: number = 12,
    categoryId?: number
  ): Observable<any> {
    const cacheKey = `${this.KEYS.menu}_p${page}_c${categoryId ?? 0}`;

    if (!this.offlineService.isOnline) {
      const cached = this.getCache(cacheKey);
      return of(cached ?? { items: [], totalCount: 0 });
    }

    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (categoryId && categoryId > 0) {
      params = params.set('categoryId', categoryId.toString());
    }

    return this._HttpClient
      .get<any>(`${environment.baseurl}/Item/menu`, { params })
      .pipe(
        tap(data => this.saveCache(cacheKey, data)),
        catchError(() => of(this.getCache(cacheKey) ?? { items: [], totalCount: 0 }))
      );
  }

  // ── Single Item — cached per id ───────────────────────
  getitembyid(Id: number): Observable<any> {
    const cacheKey = `${this.KEYS.item}${Id}`;

    if (!this.offlineService.isOnline) {
      return of(this.getCache(cacheKey));
    }

    return this._HttpClient
      .get<any>(`${environment.baseurl}/Item/${Id}`)
      .pipe(
        tap(data => this.saveCache(cacheKey, data)),
        catchError(() => of(this.getCache(cacheKey)))
      );
  }

  // ── Admin methods — disabled offline ─────────────────
  deleteItem(id: number): Observable<void> {
    if (!this.offlineService.isOnline) return of(undefined);
    return this._HttpClient.delete<void>(`${environment.baseurl}/Item/${id}`);
  }

  addtem(item: FormData): Observable<Item> {
    if (!this.offlineService.isOnline) return of({} as Item);
    return this._HttpClient.post<Item>(
      `${environment.baseurl}/Item/CreateItem`, item
    );
  }

  updateItem(id: number, data: FormData): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this._HttpClient.put(`${environment.baseurl}/Item/${id}`, data);
  }

  getItemsByCategoryId(catId: number): Observable<Item[]> {
    const cacheKey = `lunamaro_cat_items_${catId}`;

    if (!this.offlineService.isOnline) {
      return of(this.getCache(cacheKey) ?? []);
    }

    return this._HttpClient
      .get<Item[]>(`${environment.baseurl}/Item/GetItemsByCategory/${catId}`)
      .pipe(
        tap(data => this.saveCache(cacheKey, data)),
        catchError(() => of(this.getCache(cacheKey) ?? []))
      );
  }

  getPaginatedItems(
    page: number,
    pageSize: number,
    categoryId?: number,
    search?: string
  ): Observable<{ items: Item[], totalCount: number, totalPages: number, currentPage: number }> {
    const cacheKey = `lunamaro_paginated_p${page}_c${categoryId ?? 0}_q${search ?? ''}`;
    const empty    = { items: [], totalCount: 0, totalPages: 0, currentPage: page };

    if (!this.offlineService.isOnline) {
      return of(this.getCache(cacheKey) ?? empty);
    }

    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (categoryId && categoryId > 0) params = params.set('categoryId', categoryId);
    if (search?.trim())                params = params.set('search', search.trim());

    return this._HttpClient
      .get<any>(`${environment.baseurl}/Item/ItemsFilters`, { params })
      .pipe(
        tap(data => this.saveCache(cacheKey, data)),
        catchError(() => of(this.getCache(cacheKey) ?? empty))
      );
  }

  // ── Cache helpers ─────────────────────────────────────
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