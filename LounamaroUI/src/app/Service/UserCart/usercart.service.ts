import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, of, catchError } from 'rxjs';
import { Usercart } from '../../Models/usercart';
import { environment } from '../../../environments/environment.prod';
import { AddToCart } from '../../Models/add-to-cart';
import { Updatequantity } from '../../Models/updatequantity';
import { AuthService } from '../auth.service';
import { CartItem } from 'src/app/Models/UserCart/CartItems';
import { OfflineService } from '../OfflineSerivce/offline-service.service';

@Injectable({ providedIn: 'root' })
export class UsercartService {
  private loadcount  = new BehaviorSubject<number>(0);
  loadcount$         = this.loadcount.asObservable();
  private isFetching = false;
  private readonly CART_KEY = 'lunamaro_cart_cache';

  constructor(
    private _HttpClient: HttpClient,
    private authservice: AuthService,
    private offlineService: OfflineService
  ) {
    this.fetchCartCount();
  }

  // ✅ Disabled offline — shows cached count only
  addToCart(dto: AddToCart): Observable<string> {
    if (!this.offlineService.isOnline) return of('offline');
    return this._HttpClient.post<string>(
      `${environment.baseurl}/UserCart/add`, dto
    ).pipe(tap(() => this.fetchCartCount()));
  }

  resetCartCount() { this.loadcount.next(0); }

  fetchCartCount() {
    if (!this.authservice.isLoggedIn()) return;
    if (this.isFetching) return;

    // Offline — keep current count from cache
    if (!this.offlineService.isOnline) {
      const cached = localStorage.getItem(this.CART_KEY);
      if (cached) this.loadcount.next(+cached);
      return;
    }

    this.isFetching = true;
    this._HttpClient.get<number>(`${environment.baseurl}/UserCart/count`)
      .subscribe({
        next: count => {
          this.loadcount.next(count);
          localStorage.setItem(this.CART_KEY, String(count)); // save for offline
          this.isFetching = false;
        },
        error: () => {
          this.loadcount.next(0);
          this.isFetching = false;
        }
      });
  }

  // ✅ Returns cached cart items when offline
  getCartItems(): Observable<Usercart[]> {
    if (!this.offlineService.isOnline) {
      try {
        const cached = localStorage.getItem('lunamaro_cart_items');
        return of(cached ? JSON.parse(cached) : []);
      } catch { return of([]); }
    }
    return this._HttpClient.get<Usercart[]>(
      `${environment.baseurl}/UserCart/mycart`
    ).pipe(
      tap(items => localStorage.setItem('lunamaro_cart_items', JSON.stringify(items))),
      catchError(() => of([]))
    );
  }

  // ✅ Disabled offline
  deleteCart(cartItemId: number): Observable<void> {
    if (!this.offlineService.isOnline) return of(undefined);
    return this._HttpClient.delete<void>(
      `${environment.baseurl}/UserCart/remove/${cartItemId}`
    ).pipe(tap(() => this.fetchCartCount()));
  }

  // ✅ Disabled offline
  updatequantity(updatedto: Updatequantity): Observable<number> {
    if (!this.offlineService.isOnline) return of(0);
    return this._HttpClient.post<number>(
      `${environment.baseurl}/UserCart/update-quantity`, updatedto
    ).pipe(tap(() => this.fetchCartCount()));
  }

  updateCartAddOns(userCartId: number, addOnIds: number[]): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this._HttpClient.put(
      `${environment.baseurl}/UserCart/UpdateAddOns`,
      { userCartId, addOnIds }
    );
  }

getCart() {
  return this._HttpClient.get<CartItem[]>(`${environment.baseurl}/UserCart/v2`);
}

 getSuggestions() {
  return this._HttpClient.get<any[]>(`${environment.baseurl}/UserCart/suggestions2`);
}

  addToCart2(dto: any) {
    if (!this.offlineService.isOnline) return of(null);
    return this._HttpClient.post(`${environment.baseurl}/UserCart/AddtoCartv2`, dto);
  }
}