import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { Document } from 'yaml';
import { SearchResult } from '../../common/search.model';
import { StateMachine } from '../models/statemachine.model';
import { StateMachineAdded } from '../models/statemachineadded.model';

@Injectable()
export class StateMachinesService {
  constructor(
    private http: HttpClient) { }

  search(startIndex: number, count: number, order: string, direction: string, vpn: string): Observable<SearchResult<StateMachine>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/.search";
    return this.http.post<SearchResult<StateMachine>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction,
      vpn: vpn
    }, { headers: headers });
  }

  getJson(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/" + id;
    return this.http.get<any>(targetUrl);
  }

  addEmpty(name: string, description: string, vpn: string): Observable<{ id: string }> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/empty";
    return this.http.post<{ id: string }>(targetUrl, {
      name: name,
      description: description,
      vpn: vpn
    }, { headers: headers });
  }

  update(id: string, stateMachine: any): Observable<StateMachineAdded> {
    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'text/yaml');
    let targetUrl = environment.apiUrl + "/statemachines/" + id;
    const json = {
      workflowDefinition: stateMachine
    };
    const doc = new Document();
    doc.contents = json;
    const yaml = doc.toString();
    return this.http.put<StateMachineAdded>(targetUrl, yaml, { headers: headers });
  }

  launch(id: string, input: any, parameters: any): Observable<{ id: string, launchDateTime: Date }> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/statemachines/start";
    return this.http.post<{ id: string, launchDateTime: Date }>(targetUrl, {
      id: id,
      input: input,
      parameters: parameters
    }, { headers: headers });
  }
}
