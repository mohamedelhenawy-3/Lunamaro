import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, count, Observable, Observer } from 'rxjs';
import { Usercart } from '../../Models/usercart';
import { environment } from '../../../environments/environment.development';
import { AddToCart } from '../../Models/add-to-cart';
import { Updatequantity } from '../../Models/updatequantity';

@Injectable({
  providedIn: 'root'
})
export class UsercartService {

  private loadcount=new BehaviorSubject<number>(0);
  loadcount$=this.loadcount.asObservable();
  constructor(private _HttpClient:HttpClient) { }



  getCartItems():Observable<Usercart[]>{
    return this._HttpClient.get<Usercart[]>(`${environment}/UserCart/mycart`)
  }

  deleteCart(cartItemId: number): Observable<void> {
  return this._HttpClient.delete<void>(`${environment.baseurl}/UserCart/remove/${cartItemId}`);
}


  addToCart(dto: AddToCart): Observable<string> {
    return this._HttpClient.post<string>(`${environment.baseurl}/UserCart/add`, dto);
  }


  updatequantity(updatedto:Updatequantity):Observable<number>{
    return this._HttpClient.post<number>(`${environment.baseurl}/UserCart/update-quantity`, updatedto);
  }
  fetchCartCount(): void {
    this._HttpClient.get<number>(`${environment.baseurl}/UserCart/count`)
      .subscribe(count => this.loadcount.next(count)
    );
  }



}
