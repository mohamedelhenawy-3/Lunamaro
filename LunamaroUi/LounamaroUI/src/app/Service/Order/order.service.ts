import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderDetails } from '../../Models/order-details';
import { environment } from '../../../environments/environment.development';
import { OrderItem } from '../../Models/order-item';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
constructor(private _HttpClient:HttpClient) {

   }

   GetOrderPerview():Observable<OrderDetails>{
       return this._HttpClient.get<OrderDetails>(`${environment.baseurl}/Order/preview`)
     }
    GetUserCartItems():Observable<OrderItem[]>{
       return this._HttpClient.get<OrderItem[]>(`${environment.baseurl}/Order/preview-cart`)
    }
}
