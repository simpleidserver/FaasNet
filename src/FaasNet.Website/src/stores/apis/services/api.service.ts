import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { SearchResult } from '../../common/search.model';
import { ApiDefinitionOperationUIResult, ApiDefinitionResult } from '../models/apidef.model';

@Injectable()
export class ApiDefService {
  constructor(
    private http: HttpClient) { }

  add(name: string, path: string): Observable<ApiDefinitionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/apis";
    return this.http.post<any>(targetUrl, { name: name, path: path}, { headers: headers });
  }

  addOperation(funcName: string, opName: string, opPath: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/apis/" + funcName + "/operations";
    return this.http.post<any>(targetUrl, { opName: opName, opPath: opPath }, { headers: headers });
  }

  updateUIOperation(funcName: string, opName: string, ui: ApiDefinitionOperationUIResult): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/apis/" + funcName + "/operations/" + opName + "/ui";
    return this.http.put<any>(targetUrl, { operationUI: ui }, { headers: headers });
  }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<ApiDefinitionResult>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/apis/.search";
    return this.http.post<SearchResult<ApiDefinitionResult>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction
    }, { headers: headers });
  }

  get(funcName: string): Observable<ApiDefinitionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/apis/" + funcName;
    return this.http.get<ApiDefinitionResult>(targetUrl, { headers: headers });
  }
}
