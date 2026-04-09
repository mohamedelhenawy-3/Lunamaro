import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Category } from '../../Models/category';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private HttpClient:HttpClient) { }



  getallCategories():Observable<Category[]>{
    return this.HttpClient.get<Category[]>(`${environment.baseurl}/Category`)
  }

  getCategoryById(id:number):Observable<Category>{
    return this.HttpClient.get<Category>(`${environment.baseurl}/category/${id}`);
  }


  createCategory(category:Partial<Category>):Observable<Category>{
      return this.HttpClient.post<Category>(`${environment.baseurl}/Category/CreateCategory`, category);
  }
deleteCategory(id: number): Observable<void> {
  return this.HttpClient.delete<void>(`${environment.baseurl}/Category/${id}`);
}

}
