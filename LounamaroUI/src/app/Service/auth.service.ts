import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterRequest } from '../Models/User/register-request';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, Observable } from 'rxjs';
import { JwtPayload } from '../Models/jwt-payload';
import { jwtDecode } from 'jwt-decode';
import { LoginResponse } from '../Models/User/login-response';
import { GeneratedeviceIdService } from './DeviceId/generatedevice-id.service';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
private accessTokenKey = 'access_token';
private refreshTokenKey = 'refresh_token';
      private loggedIn = new BehaviorSubject<boolean>(this.checkIsLoggedIn());
        public isLoggedIn$ = this.loggedIn.asObservable();
 constructor(private http: HttpClient,  private deviceService: GeneratedeviceIdService
) { }

  register(data: RegisterRequest): Observable<any> {
    console.log(data)
    return this.http.post(`${environment.baseurl}/Auth/register`, data);
  }
  setToken(access_token:string,refresh_token:string){
    localStorage.setItem(this.accessTokenKey, access_token);
  localStorage.setItem(this.refreshTokenKey,refresh_token);
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
      this.loggedIn.next(true);
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
  this.loggedIn.next(false);
}




  // Logout
  logout(): void {
  const refreshToken = this.getRefreshToken();
  const deviceId = this.deviceService.getDeviceId();
  if (refreshToken) {
    this.http.post(
      `${environment.baseurl}/Auth/logout`,
      { refreshToken, deviceId }
    ).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession() // even if backend fails
    });
  } else {
    this.clearSession();
  }
  }
}
