import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { SearchResult } from '../../common/search.model';
import { StateMachine } from '../models/statemachine.model';

@Injectable()
export class StateMachinesService {
  constructor(
    private http: HttpClient) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<StateMachine>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/.search";
    return this.http.post<SearchResult<StateMachine>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction
    }, { headers: headers });
  }

  getJson(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/" + id;
    return this.http.get<any>(targetUrl);
  }

  addEmpty(name: string, description: string): Observable<{ id: string }> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/empty";
    console.log({
      name: name,
      description: description
    });
    return this.http.post<{ id: string }>(targetUrl, {
      name: name,
      description: description
    }, { headers: headers });
  }
}
