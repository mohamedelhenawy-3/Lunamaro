import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Item } from '../../Models/item';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { ExploreItem } from '../../Models/item/exploreItem';
import { UpdateItem } from '../../Models/item/UpdateItem';
import { ReturnedItem } from '../../Models/item/returnedItem';

@Injectable({
  providedIn: 'root'
})
export class ItemService {

  constructor(private _HttpClient:HttpClient) { }

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
getBestSelerItems():Observable<ExploreItem[]>{
    return this._HttpClient.get<ExploreItem[]>(`${environment.baseurl}/Item/popular`)

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

}
