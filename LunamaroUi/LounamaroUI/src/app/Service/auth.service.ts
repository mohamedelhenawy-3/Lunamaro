import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { RegisterRequest } from '../Models/User/register-request';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
 constructor(private http: HttpClient) { }

  register(data: RegisterRequest): Observable<any> {
    console.log(data)
    return this.http.post(`${environment.baseurl}/Auth/register`, data);
  }
  setToken(token:string){
    localStorage.setItem('token',token)
  }
   getToken(): string | null {
    return localStorage.getItem('token');
  }

  login(data: { email: string; password: string }): Observable<any> {
  return this.http.post(`${environment.baseurl}/Auth/login`, data);
}


    getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const decoded: any = jwtDecode(token);
    return decoded.role || null;
  }
    isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout() {
    localStorage.removeItem('token');
  }
}
