import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) {}

  createUser(userData: any): Observable<any> {
    return this.http.post('http://localhost:5146/api/users/create-user', userData);
  }
}
