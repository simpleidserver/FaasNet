import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { AppDomainResult } from '../models/appdomain.model';
import { ApplicationResult } from '../models/application.model';
import { ApplicationDomainAddedResult } from '../models/applicationdomainadded.model';

@Injectable()
export class ApplicationDomainService {
  constructor(
    private http: HttpClient) { }

  addAppDomain(vpn: string, name: string, description: string, rootTopic: string): Observable<ApplicationDomainAddedResult> {
    let targetUrl = environment.apiUrl + "/domains";
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      vpn: vpn,
      name: name,
      description: description,
      rootTopic: rootTopic
    };
    return this.http.post<ApplicationDomainAddedResult>(targetUrl, json, { headers: headers });
  }

  updateApplicationDomain(id: string, applications: ApplicationResult[]): Observable<any> {
    let targetUrl = environment.apiUrl + "/domains/" + id;
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      applications: applications
    };
    return this.http.put<any>(targetUrl, json, { headers: headers });
  }

  deleteApplicationDomain(id: string): Observable<any> {
    let targetUrl = environment.apiUrl + "/domains/" + id;
    return this.http.delete<any>(targetUrl);
  }

  getAppDomains(vpn: string): Observable<AppDomainResult[]> {
    let targetUrl = environment.apiUrl + "/domains/.search/" + vpn;
    return this.http.get<AppDomainResult[]>(targetUrl);
  }

  getAppDomain(id : string): Observable<AppDomainResult> {
    let targetUrl = environment.apiUrl + "/domains/" + id;
    return this.http.get<AppDomainResult>(targetUrl);
  }
}
