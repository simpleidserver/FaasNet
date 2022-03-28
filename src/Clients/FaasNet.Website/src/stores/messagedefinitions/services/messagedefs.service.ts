import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { MessageDefinitionResult } from '../models/messagedefinition.model';
import { MessageDefinitionAddedResult } from '../models/messagedefinitionadded.model';

@Injectable()
export class MessageDefsService {
  constructor(
    private http: HttpClient) { }

  getLatestMessagesDef(appDomainId: string) : Observable<MessageDefinitionResult[]> {
    let targetUrl = environment.apiUrl + "/messagedefs/.search/" + appDomainId + "/latest";
    return this.http.get<MessageDefinitionResult[]>(targetUrl);
  }

  addMessageDef(applicationDomainId: string, name: string, description: string, jsonSchema: string): Observable<MessageDefinitionAddedResult> {
    let targetUrl = environment.apiUrl + "/messagedefs";
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      applicationDomainId: applicationDomainId,
      name: name,
      description: description,
      jsonSchema: jsonSchema
    };
    return this.http.post<MessageDefinitionAddedResult>(targetUrl, json, { headers: headers });
  }

  updateMessageDef(id: string, description: string, jsonSchema: string) : Observable<any> {
    let targetUrl = environment.apiUrl + "/messagedefs/" + id;
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      description: description,
      jsonSchema: jsonSchema
    };
    return this.http.put<any>(targetUrl, json, { headers: headers });
  }

  publishMessageDef(id: string) : Observable<MessageDefinitionAddedResult> {
    let targetUrl = environment.apiUrl + "/messagedefs/" + id + "/publish";
    return this.http.get<MessageDefinitionAddedResult>(targetUrl);
  }
}
