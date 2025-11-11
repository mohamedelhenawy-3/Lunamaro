import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderDetails } from '../../Models/order-details';
import { environment } from '../../../environments/environment.development';
import { OrderItem } from '../../Models/order-item';
import { OrderDto } from '../../Models/orderDto';
import { OrderInfo } from '../../Models/User/orderUserInfo';
import { OrderRes } from '../../Models/User/orderres';
import { userorderhostory } from '../../Models/userorderhistory';
import { OrderHistoryDetails } from '../../Models/orderdetails';
import { orderhistory } from '../../Models/Admin/Orders/orderhistory';
import { OrderStatus } from '../../Models/orderStatus';

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
orderhistory():Observable<userorderhostory[]>{
    return this._HttpClient.get<userorderhostory[]>(`${environment.baseurl}/Order/history`)
}
getOrderHistoryDetails(orderId: number) {
 return this._HttpClient.get<OrderHistoryDetails>(`${environment.baseurl}/Order/history/${orderId}`);

}
getOrderHistoryDetailsAd(orderId: number) {
 return this._HttpClient.get<OrderHistoryDetails>(`${environment.baseurl}/Order/historyAd/${orderId}`);

}


//Admin
getOrders():Observable<orderhistory[]>{
  return this._HttpClient.get<orderhistory[]>(`${environment.baseurl}/Order/AllOrders`)
}
updateOrderStatus(orderId: number, status: OrderStatus) {
  return this._HttpClient.post(`${environment.baseurl}/Order/${orderId}/update-status`, { status });
}
}
