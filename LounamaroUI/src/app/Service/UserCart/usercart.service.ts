import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, count, Observable, Observer, tap } from 'rxjs';
import { Usercart } from '../../Models/usercart';
import { environment } from '../../../environments/environment.prod';
import { AddToCart } from '../../Models/add-to-cart';
import { Updatequantity } from '../../Models/updatequantity';
import { AuthService } from '../auth.service';
import { CartItem } from 'src/app/Models/UserCart/CartItems';

@Injectable({
  providedIn: 'root'
})
export class UsercartService {


  private loadcount=new BehaviorSubject<number>(0);
  loadcount$=this.loadcount.asObservable();
  constructor(private _HttpClient:HttpClient,private authservice:AuthService) {
    this.fetchCartCount();
   }

  addToCart(dto: AddToCart): Observable<string> {
    return this._HttpClient.post<string>(`${environment.baseurl}/UserCart/add`, dto).pipe(tap(() => this.fetchCartCount()));
  }

  resetCartCount() {
  this.loadcount.next(0); // instantly sets navbar to 0
}

fetchCartCount() {
  if (!this.authservice.isLoggedIn()) return;

  this._HttpClient.get<number>(`${environment.baseurl}/UserCart/count`)
    .subscribe({
      next: count => this.loadcount.next(count),
      error: () => this.loadcount.next(0) // prevent crash
    });
}

  
  getCartItems():Observable<Usercart[]>{
    return this._HttpClient.get<Usercart[]>(`${environment.baseurl}/UserCart/mycart`)
  }

deleteCart(cartItemId: number): Observable<void> {
  return this._HttpClient.delete<void>(`${environment.baseurl}/UserCart/remove/${cartItemId}`)
    .pipe(tap(() => this.fetchCartCount())); 
}

updatequantity(updatedto: Updatequantity): Observable<number> {
  return this._HttpClient.post<number>(`${environment.baseurl}/UserCart/update-quantity`, updatedto)
    .pipe(tap(() => this.fetchCartCount()));
}




// new v
updateCartAddOns(userCartId: number, addOnIds: number[]): Observable<any> {
  return this._HttpClient.put(
    `${environment.baseurl}/UserCart/UpdateAddOns`,
    { userCartId, addOnIds }
  );
}
getCart() {
  return this._HttpClient.get<CartItem[]>('/api/UserCart/v2');
}

getSuggestions() {
  return this._HttpClient.get<any[]>('/api/UserCart/suggestions2');
}

addToCart2(dto: any) {
  return this._HttpClient.post('/api/UserCart/AddtoCartv2', dto);
}
}
