import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Item } from '../../Models/item';
import { environment } from '../../../environments/environment.prod';
import { Observable } from 'rxjs';
import { ExploreItem } from '../../Models/item/exploreItem';
import { UpdateItem } from '../../Models/item/UpdateItem';
import { ReturnedItem } from '../../Models/item/returnedItem';
import { specialItem } from '../../Models/item/specialitems';

@Injectable({
  providedIn: 'root'
})
export class ItemService {

  constructor(private _HttpClient:HttpClient) { }

getItems(page: number = 1, pageSize: number = 12, categoryId?: number): Observable<any> {
  let params = new HttpParams()
    .set('page', page.toString())
    .set('pageSize', pageSize.toString());

  if (categoryId && categoryId > 0) {
    params = params.set('categoryId', categoryId.toString());
  }

  return this._HttpClient.get<any>(`${environment.baseurl}/Item/menu`, { params });
}
 getallItems():Observable<Item[]>{
    return this._HttpClient.get<Item[]>(`${environment.baseurl}/Item/AllNote`)
  }
deleteItem(id: number): Observable<void> {
  return this._HttpClient.delete<void>(`${environment.baseurl}/Item/${id}`);
}
addtem(item:FormData):Observable<Item>{
  return this._HttpClient.post<Item>(`${environment.baseurl}/Item/CreateItem`,item);
}

getItemsByCategoryId(catId:number):Observable<Item[]>{
  return this._HttpClient.get<Item[]>(`${environment.baseurl}/Item/GetItemsByCategory/${catId}`)
}
getBestSelerItems():Observable<any[]>{
    return this._HttpClient.get<any[]>(`${environment.baseurl}/Item/popular`)

}
getNewestItems():Observable<ExploreItem[]>{
    return this._HttpClient.get<ExploreItem[]>(`${environment.baseurl}/Item/menu-preview`)
}
getitembyid(Id:number):Observable<ReturnedItem>{
    return this._HttpClient.get<ReturnedItem>(`${environment.baseurl}/Item/${Id}`)

}

updateItem(id: number, data: FormData): Observable<any> {
  return this._HttpClient.put(`${environment.baseurl}/Item/${id}`, data);
}
getSpecialItems() {
  return this._HttpClient.get<specialItem[]>(`${environment.baseurl}/Item/SpecialItems`);
}
getPaginatedItems(
  page: number,
  pageSize: number,
  categoryId?: number,
  search?: string
): Observable<{ items: Item[], totalCount: number, totalPages: number, currentPage: number }> {

  let params = new HttpParams()
    .set('page', page)
    .set('pageSize', pageSize);

  if (categoryId && categoryId > 0) {
    params = params.set('categoryId', categoryId);
  }

  if (search && search.trim()) {
    params = params.set('search', search.trim());
  }

  return this._HttpClient.get<any>(`${environment.baseurl}/Item/ItemsFilters`, { params });
}
}
