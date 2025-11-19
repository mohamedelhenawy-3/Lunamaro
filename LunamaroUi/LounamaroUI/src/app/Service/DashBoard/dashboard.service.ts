import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { HeaderStatsDTO } from '../../Models/dashboard/headerstate';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

   constructor(private _httpclient:HttpClient) { }


    getHeaderStats():Observable<HeaderStatsDTO>{
        return this._httpclient.get<HeaderStatsDTO>(`${environment.baseurl}/Dashboard/header-stats`);
     }




  getRevenueLast7Days(): Observable<any> {
    return this._httpclient.get(`${environment.baseurl}/Dashboard/revenue-chart`);
  }
  
  getMonthlyOrders(): Observable<any> {
    return this._httpclient.get(`${environment.baseurl}/Dashboard/monthly-orders-chart`);
  }
  getProductCategories(): Observable<any> {
    return this._httpclient.get(`${environment.baseurl}/Dashboard/product-categories-chart`);
  }
getRecentOrders() :Observable<any>{
  return this._httpclient.get(`${environment.baseurl}/Dashboard/recent-orders`);
}

}
