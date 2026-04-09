import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GeneratedeviceIdService {
private readonly devicekey="device_id";




getDeviceId():string{
  let device_id=localStorage.getItem(this.devicekey);
  if(!device_id){
    device_id=crypto.randomUUID();
    localStorage.setItem(this.devicekey,device_id);
  }


  return device_id;
}
 

}
