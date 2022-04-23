import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { SearchResult } from '../../common/search.model';
import { StateMachineInstanceDetails } from '../models/statemachineinstance-details.model';
import { StateMachineInstance } from '../models/statemachineinstance.model';

@Injectable()
export class StateMachineInstancesService {
  constructor(
    private http: HttpClient) { }

  search(startIndex: number, count: number, order: string, direction: string, vpn: string): Observable<SearchResult<StateMachineInstance>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachineinstances/.search";
    return this.http.post<SearchResult<StateMachineInstance>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction,
      vpn: vpn
    }, { headers: headers });
  }

  get(id: string): Observable<StateMachineInstanceDetails> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachineinstances/" + id;
    return this.http.get<StateMachineInstanceDetails>(targetUrl, { headers: headers });
  }

  reactivate(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachineinstances/" + id + "/reactivate";
    return this.http.get<any>(targetUrl, { headers: headers });
  }
}
