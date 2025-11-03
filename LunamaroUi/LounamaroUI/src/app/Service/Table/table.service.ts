import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Table } from '../../Models/tables';
import { environment } from '../../../environments/environment.development';
import { updatetablestatus } from '../../Models/updatetablestatus';

@Injectable({
  providedIn: 'root'
})
export class TableService {

  constructor(private http:HttpClient) { }


  getAllTables():Observable<Table[]>{
     return this.http.get<Table[]>(`${environment.baseurl}/table`);
  }

    UpdatetTableStatus(id: number, dto:updatetablestatus):Observable<any>{
      return this.http.put(`${environment.baseurl}/table/${id}/status`,dto);
    }
}
