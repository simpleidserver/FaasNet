import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { VpnResult } from '../models/vpn.model';

@Injectable()
export class VpnService {
  constructor(
    private http: HttpClient) { }

  getAllVpn(): Observable<VpnResult[]> {
    let targetUrl = environment.apiUrl + "/vpns";
    return this.http.get<VpnResult[]>(targetUrl);
  }

  addVpn(name: string, description: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/vpns";
    return this.http.post<{ id: string }>(targetUrl, {
      name: name,
      description: description
    }, { headers: headers });
  }

  addBridge(vpn: string, urn: string, port: number, targetVpn: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/bridges';
    const json = {
      urn: urn,
      port: port,
      targetVpn: targetVpn
    };
    return this.http.post<any>(targetUrl, json, { headers: headers });
  }

  deleteVpn(vpn: string): Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn;
    return this.http.delete<any>(targetUrl);
  }

  getVpn(vpn: string): Observable<VpnResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn;
    return this.http.get<VpnResult>(targetUrl);
  }
}
