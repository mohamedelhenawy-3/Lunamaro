import { HttpClient, HttpParams } from '@angular/common/http';
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

    getWeeklyDealById(id:number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getweakdeal/${id}`);
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
   searchProducts(term: string) {
    let params = new HttpParams().set('term', term);
    return this.http.get<any[]>(`${environment.baseurl}/admin/offers/products/search`, { params });
  }

  updateWeeklyDeal(id:number, data:any) {
    return this.http.put(`${environment.baseurl}/admin/offers/weekly-deals/${id}`, data);
  }

  
  UpdateDiscountTiers(id:number, data:any) {
    return this.http.put(`${environment.baseurl}/admin/offers/discount-tiers/${id}`, data);
  }
   UpdateAddOnRewards(id:number, data:any) {
    return this.http.put(`${environment.baseurl}/admin/offers/UpdateAddOnReward/${id}`, data);
  }


  deleteWeeklyDeal(id:number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/weekly-deals/${id}`);
  }

  getDiscountTiers() {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/discount-tiers`);
  }
  getDiscountTiersById(id:number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getdiscounttierbyid/${id}`);
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
 getAddOnRewardById(id:number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getadd-on-reward/${id}`);
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
