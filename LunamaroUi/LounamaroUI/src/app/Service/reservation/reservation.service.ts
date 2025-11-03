import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Reservation } from '../../Models/reservation';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { RecievedReservation } from '../../Models/Reseviedreservations';
import { UpdateStatus } from '../../Models/updateStatus';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor(private httpclient:HttpClient) { }

  addreservation(reservation:Reservation):Observable<any>{
    return this.httpclient.post(`${environment.baseurl}/Reservation`,reservation);
  }
  AllRecervations():Observable<RecievedReservation[]>{
      return this.httpclient.get<RecievedReservation[]>(`${environment.baseurl}/Reservation`);
  }
  UpdateReservation(id: number, dto:UpdateStatus):Observable<any>{
    return this.httpclient.put(`${environment.baseurl}/Reservation/${id}/status`,dto);
  }
}
