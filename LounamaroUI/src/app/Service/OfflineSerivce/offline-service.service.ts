import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent, merge, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OfflineService {

  private onlineSubject = new BehaviorSubject<boolean>(navigator.onLine);

  isOnline$ = this.onlineSubject.asObservable();

  get isOnline(): boolean {
    return this.onlineSubject.value;
  }

  constructor() {
    merge(
      fromEvent(window, 'online').pipe(map(() => true)),
      fromEvent(window, 'offline').pipe(map(() => false))
    ).subscribe(status => {
      this.onlineSubject.next(status);
    });
  }
}