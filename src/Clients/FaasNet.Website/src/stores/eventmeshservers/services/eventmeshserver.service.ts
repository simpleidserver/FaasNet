import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { EventMeshServerResult } from '../models/eventmeshserver.model';

@Injectable()
export class EventMeshServerService {
  constructor(
    private http: HttpClient) { }

  add(isLocalhost: boolean, urn: string, port: number): Observable<EventMeshServerResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/eventmesh";
    return this.http.post<EventMeshServerResult>(targetUrl, { isLocalhost: isLocalhost, urn: urn, port: port }, { headers: headers });
  }

  getAll(): Observable<EventMeshServerResult[]> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/eventmesh";
    return this.http.get<EventMeshServerResult[]>(targetUrl, { headers: headers });
  }

  addBridge(from: string, fromPort: number, to: string, toPort: number) : Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/eventmesh/bridge";
    return this.http.post<any>(targetUrl, { from: { urn: from, port: fromPort }, to: { urn: to, port: toPort }}, { headers: headers })
  }
}
