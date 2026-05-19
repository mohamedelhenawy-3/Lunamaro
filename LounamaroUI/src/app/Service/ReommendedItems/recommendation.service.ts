import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class RecommendationService {


  constructor(private http: HttpClient) { }

  getSuggestions(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.baseurl}/UserCart/suggestions2`);
  }
}
