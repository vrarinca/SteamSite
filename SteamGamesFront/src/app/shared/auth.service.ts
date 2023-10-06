// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  url:string  = environment.apiBaseUrl + '/api/UserCredentials/login';
  constructor(private http: HttpClient) {}

  login(email: string, password: string) {
    const credentials = { email, password };
    
    return this.http.post(`${this.url}`, credentials);
  }

  isLoggedIn() {
    return !!localStorage.getItem('token');
  }
}