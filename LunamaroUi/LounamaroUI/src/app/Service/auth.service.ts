import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterRequest } from '../Models/User/register-request';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, Observable } from 'rxjs';
import { JwtPayload } from '../Models/jwt-payload';
import { jwtDecode } from 'jwt-decode';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private tokenKey = 'token';
      private loggedIn = new BehaviorSubject<boolean>(this.checkIsLoggedIn());
        public isLoggedIn$ = this.loggedIn.asObservable();
 constructor(private http: HttpClient) { }

  register(data: RegisterRequest): Observable<any> {
    console.log(data)
    return this.http.post(`${environment.baseurl}/Auth/register`, data);
  }
  setToken(token:string){
    localStorage.setItem(this.tokenKey,token)
  }
   getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  login(data: { email: string; password: string }): Observable<any> {
    return new Observable(observer => {
      this.http.post<{ token: string }>(`${environment.baseurl}/Auth/login`, data).subscribe({
        next: (response) => {
          const token = response.token;
          this.setToken(token);
          this.loggedIn.next(true);
          observer.next(response);
          observer.complete();
        },
        error: (err) => {
          observer.error(err);
        }
      });
    });
  }

getPayload():JwtPayload | null {
    const token = this.getToken();
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

  // Logout
  logout(): void {
     localStorage.removeItem(this.tokenKey);
    this.loggedIn.next(false);
  }
}
