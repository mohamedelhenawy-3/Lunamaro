import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class OffersservicesService {

 constructor(private http:HttpClient) {
 
    }
  getWeeklyDeals() {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/weekly-deals`);
  }
activateWeeklyDeal(id: number) {
  return this.http.patch(`${environment.baseurl}/admin/offers/weekly-deals/${id}/activate`, {});
}

deactivateWeeklyDeal(id: number) {
  return this.http.patch(`${environment.baseurl}/admin/offers/weekly-deals/${id}/deactivate`, {});
}
  createWeeklyDeal(data:any) {
    return this.http.post(`${environment.baseurl}/admin/offers/weekly-deals`, data);
  }

  updateWeeklyDeal(id:number, data:any) {
    return this.http.put(`${environment.baseurl}/admin/offers/weekly-deals/${id}`, data);
  }

  deleteWeeklyDeal(id:number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/weekly-deals/${id}`);
  }

  // Discount Tiers
  getDiscountTiers() {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/discount-tiers`);
  }

  createDiscountTier(data:any) {
    return this.http.post(`${environment.baseurl}/admin/offers/discount-tiers`, data);
  }

  deleteDiscountTier(id:number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/discount-tiers/${id}`);
  }
    activateDiscountTier(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/discount-tiers/${id}/activate`, {});
  }

  deactivateDiscountTier(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/discount-tiers/${id}/deactivate`, {});
  }

  // Add-On Rewards
  getAddOnRewards() {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/add-on-rewards`);
  }

  createAddOnReward(data:any) {
    return this.http.post(`${environment.baseurl}/admin/offers/add-on-rewards`, data);
  }

  deleteAddOnReward(id:number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/add-on-rewards/${id}`);
  }
    activateAddOnReward(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/add-on-rewards/${id}/activate`, {});
  }

  deactivateAddOnReward(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/add-on-rewards/${id}/deactivate`, {});
  }

}
