import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Item } from '../../Models/item';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';

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
  
}
