import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { SearchResult } from '../../common/search.model';
import { FunctionResult } from '../models/function.model';

@Injectable()
export class FunctionService {
  constructor(
    private http: HttpClient) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<FunctionResult>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/.search";
    return this.http.post<SearchResult<FunctionResult>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction
    }, { headers: headers });
  }

  getConfiguration(name: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + '/configuration';
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  invoke(name: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + '/invoke';
    return this.http.post<any>(targetUrl, request, { headers: headers });
  }

  get(name: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name;
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  delete(name: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name;
    return this.http.delete<any>(targetUrl, { headers: headers });
  }

  add(name: string, image: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions";
    return this.http.post<any>(targetUrl, { name: name, image: image }, { headers: headers });
  }
}
