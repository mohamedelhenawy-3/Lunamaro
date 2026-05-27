import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Reservation } from '../../Models/reservation';
import { Observable, of, catchError, tap } from 'rxjs';
import { environment } from '../../../environments/environment.prod';
import { RecievedReservation } from '../../Models/Reseviedreservations';
import { UpdateStatus } from '../../Models/updateStatus';
import { userrecervation } from '../../Models/User/userreciervation';
import { OfflineService } from '../OfflineSerivce/offline-service.service';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly MY_RESERVATIONS_KEY = 'lunamaro_my_reservations_cache';
  private readonly ALL_RESERVATIONS_KEY = 'lunamaro_all_reservations_cache';

  constructor(
    private httpclient: HttpClient,
    private offlineService: OfflineService
  ) {}

  // ✅ Disabled offline
  addreservation(reservation: Reservation): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this.httpclient.post(`${environment.baseurl}/Reservation`, reservation);
  }

  AllRecervations(): Observable<RecievedReservation[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.ALL_RESERVATIONS_KEY) ?? []);
    }
    return this.httpclient.get<RecievedReservation[]>(
      `${environment.baseurl}/Reservation`
    ).pipe(
      tap(data => this.saveCache(this.ALL_RESERVATIONS_KEY, data)),
      catchError(() => of(this.getCache(this.ALL_RESERVATIONS_KEY) ?? []))
    );
  }

  // ✅ Disabled offline
  UpdateReservation(id: number, dto: UpdateStatus): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this.httpclient.put(
      `${environment.baseurl}/Reservation/${id}/status`, dto
    );
  }

  UserReservation(): Observable<userrecervation[]> {
    if (!this.offlineService.isOnline) {
      return of(this.getCache(this.MY_RESERVATIONS_KEY) ?? []);
    }
    return this.httpclient.get<userrecervation[]>(
      `${environment.baseurl}/Reservation/myreservations`
    ).pipe(
      tap(data => this.saveCache(this.MY_RESERVATIONS_KEY, data)),
      catchError(() => of(this.getCache(this.MY_RESERVATIONS_KEY) ?? []))
    );
  }

  // ✅ Disabled offline
  cancelReservation(reservationId: number): Observable<any> {
    if (!this.offlineService.isOnline) return of(null);
    return this.httpclient.delete(
      `${environment.baseurl}/Reservation/cancel`,
      { body: { reservationId } }
    );
  }

  // ✅ Disabled offline — needs real-time availability
  getAvailableTables(
    startTime: string, endTime: string, guests: number
  ): Observable<any[]> {
    if (!this.offlineService.isOnline) return of([]);
    return this.httpclient.get<any[]>(
      `${environment.baseurl}/Reservation/available?startTime=${startTime}&endTime=${endTime}&guests=${guests}`
    );
  }

  private saveCache(key: string, data: any): void {
    try { localStorage.setItem(key, JSON.stringify(data)); } catch {}
  }

  private getCache(key: string): any {
    try {
      const stored = localStorage.getItem(key);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }
}