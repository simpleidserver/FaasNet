import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { ClientResult } from '../models/client.model';
import { ClientAddedResult } from '../models/clientadded.model';

@Injectable()
export class ClientService {
  constructor(
    private http: HttpClient) { }

  getAllClients(vpn : string) : Observable<ClientResult[]> {
    let targetUrl = environment.apiUrl + "/clients/.search/" + vpn;
    return this.http.get<ClientResult[]>(targetUrl);
  }

  getClient(id: string): Observable<ClientResult> {
    let targetUrl = environment.apiUrl + "/clients/" + id;
    return this.http.get<ClientResult>(targetUrl);
  }

  deleteClient(id: string): Observable<any> {
    let targetUrl = environment.apiUrl + "/clients/" + id;
    return this.http.delete<ClientResult>(targetUrl);
  }

  addClient(vpn: string, clientId: string, purposes: number[]): Observable<ClientAddedResult> {
    let targetUrl = environment.apiUrl + "/clients";
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      vpn: vpn,
      clientId: clientId,
      purposes: purposes
    };
    return this.http.post<ClientAddedResult>(targetUrl, json, { headers: headers });
  }
}
