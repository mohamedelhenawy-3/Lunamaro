import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImageShareService {
 private imageSource = new BehaviorSubject<string>('');

 currentimage= this.imageSource.asObservable();
  constructor() { }


    updateImage(newImage: string) {
    this.imageSource.next(newImage);
  }
}
