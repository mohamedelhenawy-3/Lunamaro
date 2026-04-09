import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Table } from '../../Models/tables';
import { environment } from '../../../environments/environment.development';
import { updatetablestatus } from '../../Models/updatetablestatus';
import { AvTable } from '../../Models/usersTables';
import { UpdateTable } from '../../Models/Admin/Table/updatetable';

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

  getAllAvailableTables():Observable<AvTable[]>{
     return this.http.get<AvTable[]>(`${environment.baseurl}/table/available`);
  }
 UpdatetTable(id: number, dto:UpdateTable):Observable<any>{
      return this.http.put(`${environment.baseurl}/table/${id}`,dto);
    }
getitembyid(Id:number):Observable<any>{
    return this.http.get<any>(`${environment.baseurl}/table/${Id}`)

}
AddTable(dto:any):Observable<any>{
       return this.http.post(`${environment.baseurl}/table`,dto);
}
}
