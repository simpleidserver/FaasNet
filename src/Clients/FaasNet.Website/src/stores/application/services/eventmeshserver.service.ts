import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { ApplicationDomainResult } from '../models/applicationdomain.model';

@Injectable()
export class ApplicationService {
  constructor(
    private http: HttpClient) { }

  addApplicationDomain(rootTopic: string, name: string, description: string): Observable<ApplicationDomainResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/applicationdomains";
    return this.http.post<ApplicationDomainResult>(targetUrl, { rootTopic: rootTopic, name: name, description: description }, { headers: headers });
  }

  getAllApplicationDomains(): Observable<ApplicationDomainResult[]> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/applicationdomains";
    return this.http.get<ApplicationDomainResult[]>(targetUrl, { headers: headers });
  }
}
