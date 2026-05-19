import { HttpClient } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import { RegisterRequest } from '../Models/User/register-request';
import { environment } from '../../environments/environment.prod';
import { BehaviorSubject, Observable } from 'rxjs';
import { JwtPayload } from '../Models/jwt-payload';
import { jwtDecode } from 'jwt-decode';
import { LoginResponse } from '../Models/User/login-response';
import { GeneratedeviceIdService } from './DeviceId/generatedevice-id.service';
import { tap } from 'rxjs';
import { Router } from '@angular/router';
declare var google: any;
@Injectable({
  providedIn: 'root'
})

export class AuthService {

  private isInitialized = false;
private accessTokenKey = 'access_token';
private refreshTokenKey = 'refresh_token';




 

      private loggedIn = new BehaviorSubject<boolean>(this.checkIsLoggedIn());
        public isLoggedIn$ = this.loggedIn.asObservable();
 constructor(private http: HttpClient,private router:Router,private deviceService: GeneratedeviceIdService,private zone: NgZone
) { 
}

  register(data: any): Observable<any> {
    console.log(data)
    return this.http.post(`${environment.baseurl}/Auth/register`, data);
  }




getAccessToken(): string | null {
  return localStorage.getItem(this.accessTokenKey);
}

getRefreshToken(): string | null {
  return localStorage.getItem(this.refreshTokenKey);
}
login(data: { email: string; password: string }): Observable<LoginResponse> {
  const payload = {
    ...data,
    deviceId: this.deviceService.getDeviceId()
  };

  return this.http.post<LoginResponse>(
    `${environment.baseurl}/Auth/login`,
    payload
  ).pipe(
    tap(response => {
      this.setToken(response.accessToken, response.refreshToken);
    })
  );
}


getPayload():JwtPayload | null {
    const token = this.getAccessToken();
    if (!token) return null;

    try {
      return jwtDecode<JwtPayload>(token);
    } catch (error) {
      console.error('Invalid token:', error);
      return null;
    }
  } 
   private checkIsLoggedIn(): boolean {
    const payload = this.getPayload();
    if (!payload) return false;
    const currentTime = Math.floor(Date.now() / 1000);
    return payload.exp > currentTime;
  }

  getUserId(): string | null {
  const payload = this.getPayload();
  if (!payload) return null;

  const userIdClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
  return (payload as any)[userIdClaim] || null;
}

  
  isLoggedIn(): boolean {
    return this.checkIsLoggedIn();
  }
getUserRole(): string | null {
  const payload = this.getPayload();
  if (!payload) return null;
  const roleClaimKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  return (payload as any)[roleClaimKey] || null;
}



refreshToken(): Observable<LoginResponse> {
  return this.http.post<LoginResponse>(
    `${environment.baseurl}/Auth/refresh`,
    {
      refreshToken: this.getRefreshToken(),
      deviceId: this.deviceService.getDeviceId()
    }
  );
}


private clearSession() {
  localStorage.removeItem(this.accessTokenKey);
  localStorage.removeItem(this.refreshTokenKey);
  this.zone.run(() => this.loggedIn.next(false)); // ✅ forces Angular to detect
}

setToken(access_token: string, refresh_token: string) {
  localStorage.setItem(this.accessTokenKey, access_token);
  localStorage.setItem(this.refreshTokenKey, refresh_token);
  this.zone.run(() => this.loggedIn.next(true)); // ✅ same fix for login
}

// loginWithSocial(provider: string, token: string): Observable<any> {
//   return this.http.post<any>(`${environment.baseurl}/Auth/social-login`, {
//     provider: provider,
//     token: token
//   }).pipe(
//     tap(res => {
//       if (res && res.accessToken) {
//         this.setToken(res.accessToken, res.refreshToken || '');
//         this.loggedIn.next(true);
//       } else {
//         console.error('No accessToken in response!', res);
//       }
//     })
//   );
// }
loginWithSocial(provider: string, token: string): Observable<any> {
  return this.http.post<any>(`${environment.baseurl}/Auth/social-login`, {
    provider: provider,
    token: token
  }).pipe(
    tap(res => {
      if (res && res.accessToken) {
        this.setToken(res.accessToken, res.refreshToken || '');
        this.loggedIn.next(true);
      }
    })
  );
}













  // Logout
// FIXED — clears instantly, tells backend in background
logout(): void {
  const refreshToken = this.getRefreshToken();
  const deviceId = this.deviceService.getDeviceId();

  this.clearSession();

  if (refreshToken) {
    this.http.post(
      `${environment.baseurl}/Auth/logout`,
      { refreshToken, deviceId }
    ).subscribe(); // fire and forget
  }
}
}
