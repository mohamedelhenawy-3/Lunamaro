import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class OffersservicesService {

  constructor(private http: HttpClient) {}

  // ✅ Cached — offers don't change every second
  private weeklyDeals$ = this.http
    .get<any>(`${environment.baseurl}/admin/offers/weekly-deals`)
    .pipe(shareReplay(1));

  private discountTiers$ = this.http
    .get<any>(`${environment.baseurl}/admin/offers/discount-tiers`)
    .pipe(shareReplay(1));

  private addOnRewards$ = this.http
    .get<any>(`${environment.baseurl}/admin/offers/add-on-rewards`)
    .pipe(shareReplay(1));

  // ✅ Cached getters
  getWeeklyDeals(): Observable<any> {
    return this.weeklyDeals$;
  }

  getDiscountTiers(): Observable<any> {
    return this.discountTiers$;
  }

  getAddOnRewards(): Observable<any> {
    return this.addOnRewards$;
  }

  // ✅ NOT cached — per id lookups
  getWeeklyDealById(id: number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getweakdeal/${id}`);
  }

  getDiscountTiersById(id: number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getdiscounttierbyid/${id}`);
  }

  getAddOnRewardById(id: number) {
    return this.http.get<any>(`${environment.baseurl}/admin/offers/getadd-on-reward/${id}`);
  }

  activateWeeklyDeal(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/weekly-deals/${id}/activate`, {});
  }

  deactivateWeeklyDeal(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/weekly-deals/${id}/deactivate`, {});
  }

  createWeeklyDeal(data: any) {
    return this.http.post(`${environment.baseurl}/admin/offers/weekly-deals`, data);
  }

  updateWeeklyDeal(id: number, data: any) {
    return this.http.put(`${environment.baseurl}/admin/offers/weekly-deals/${id}`, data);
  }

  deleteWeeklyDeal(id: number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/weekly-deals/${id}`);
  }

  searchProducts(term: string) {
    let params = new HttpParams().set('term', term);
    return this.http.get<any[]>(`${environment.baseurl}/admin/offers/products/search`, { params });
  }

  UpdateDiscountTiers(id: number, data: any) {
    return this.http.put(`${environment.baseurl}/admin/offers/discount-tiers/${id}`, data);
  }

  createDiscountTier(data: any) {
    return this.http.post(`${environment.baseurl}/admin/offers/discount-tiers`, data);
  }

  deleteDiscountTier(id: number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/discount-tiers/${id}`);
  }

  activateDiscountTier(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/discount-tiers/${id}/activate`, {});
  }

  deactivateDiscountTier(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/discount-tiers/${id}/deactivate`, {});
  }

  UpdateAddOnRewards(id: number, data: any) {
    return this.http.put(`${environment.baseurl}/admin/offers/UpdateAddOnReward/${id}`, data);
  }

  createAddOnReward(data: any) {
    return this.http.post(`${environment.baseurl}/admin/offers/add-on-rewards`, data);
  }

  deleteAddOnReward(id: number) {
    return this.http.delete(`${environment.baseurl}/admin/offers/add-on-rewards/${id}`);
  }

  activateAddOnReward(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/add-on-rewards/${id}/activate`, {});
  }

  deactivateAddOnReward(id: number) {
    return this.http.patch(`${environment.baseurl}/admin/offers/add-on-rewards/${id}/deactivate`, {});
  }
}