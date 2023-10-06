import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  url: string = environment.apiBaseUrl + '/api/Game';

  constructor(private http: HttpClient) {}

  getAllGames(): Observable<any[]> {
    return this.http.get<any[]>(`${this.url + '/getgames'}`);
  }

  getGameDetails(appid: number): Observable<any> {
    return this.http.get<any>(`${this.url}/getgamedetails/${appid}`);
  }
}