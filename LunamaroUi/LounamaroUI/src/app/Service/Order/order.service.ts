import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderDetails } from '../../Models/order-details';
import { environment } from '../../../environments/environment.development';
import { OrderItem } from '../../Models/order-item';
import { OrderDto } from '../../Models/orderDto';
import { OrderInfo } from '../../Models/User/orderUserInfo';
import { OrderRes } from '../../Models/User/orderres';

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


 placeOrder(order:OrderInfo) {
  return this._HttpClient.post<OrderRes>(`${environment.baseurl}/Order/create`, order);
}
paymentSuccess(sessionId: string) {
  return this._HttpClient.get(`${environment.baseurl}/Order/success?sessionId=${sessionId}`);
}

}
