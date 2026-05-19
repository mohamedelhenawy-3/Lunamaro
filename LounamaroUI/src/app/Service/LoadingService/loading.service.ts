import { Injectable, NgZone, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private count = 0;
  readonly isLoading = signal(false);

  constructor(private zone: NgZone) {}

  show() {
    this.zone.run(() => {        
      this.count++;
      this.isLoading.set(true);
    });
  }

  hide() {
    this.zone.run(() => {    
      this.count = Math.max(0, this.count - 1);
      if (this.count === 0) this.isLoading.set(false);
    });
  }

}

