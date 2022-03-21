import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { AppDomainResult } from '../models/appdomain.model';
import { ApplicationDomainAddedResult } from '../models/applicationdomainadded.model';
import { ClientResult } from '../models/client.model';
import { MessageDefinitionResult } from '../models/messagedefinition.model';
import { MessageDefinitionAddedResult } from '../models/messagedefinitionadded.model';
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

  addApplicationDomain(vpn: string, rootTopic: string, name: string, description: string): Observable<ApplicationDomainAddedResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/domains';
    const json = {
      rootTopic: rootTopic,
      name: name,
      description: description
    };
    return this.http.post<ApplicationDomainAddedResult>(targetUrl, json, { headers: headers });
  }

  deleteApplicationDomain(vpn: string, applicationDomainId : string): Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/domains/' + applicationDomainId;
    return this.http.delete<any>(targetUrl);
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

  getAllClients(vpn : string) : Observable<ClientResult[]> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients";
    return this.http.get<ClientResult[]>(targetUrl);
  }

  getClient(vpn: string, clientId: string): Observable<ClientResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients/" + clientId;
    return this.http.get<ClientResult>(targetUrl);
  }

  deleteClient(vpn: string, clientId: string): Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients/" + clientId;
    return this.http.delete<ClientResult>(targetUrl);
  }

  addClient(vpn: string, clientId: string, purposes: number[]): Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients";
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      vpn: vpn,
      clientId: clientId,
      purposes: purposes
    };
    return this.http.post<any>(targetUrl, json, { headers: headers });
  }

  addAppDomain(vpn: string, name: string, description: string, rootTopic: string): Observable<ApplicationDomainAddedResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains";
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      name: name,
      description: description,
      rootTopic: rootTopic
    };
    return this.http.post<ApplicationDomainAddedResult>(targetUrl, json, { headers: headers });
  }

  getAppDomains(vpn: string): Observable<AppDomainResult[]> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains";
    return this.http.get<AppDomainResult[]>(targetUrl);
  }

  getAppDomain(vpn: string, appDomainId: string): Observable<AppDomainResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + appDomainId;
    return this.http.get<AppDomainResult>(targetUrl);
  }

  deleteAppDomain(vpn: string, appDomainId: string): Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + appDomainId;
    return this.http.delete<any>(targetUrl);
  }

  getLatestMessagesDef(name: string, appDomainId: string) : Observable<MessageDefinitionResult[]> {
    let targetUrl = environment.apiUrl + "/vpns/" + name + "/domains/" + appDomainId + '/messages/latest';
    return this.http.get<MessageDefinitionResult[]>(targetUrl);
  }

  addMessageDef(vpn: string, applicationDomainId: string, name: string, description: string, jsonSchema: string): Observable<MessageDefinitionAddedResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + applicationDomainId + '/messages';
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      name: name,
      description: description,
      jsonSchema: jsonSchema
    };
    return this.http.post<MessageDefinitionAddedResult>(targetUrl, json, { headers: headers });
  }

  updateMessageDef(vpn: string, applicationDomainId: string, messageId: string, description: string, jsonSchema: string) : Observable<any> {
    let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + applicationDomainId + '/messages/' + messageId;
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    const json = {
      description: description,
      jsonSchema: jsonSchema
    };
    return this.http.put<any>(targetUrl, json, { headers: headers });
  }

  publishMessageDef(name: string, appDomainId: string, messageName: string) : Observable<MessageDefinitionAddedResult> {
    let targetUrl = environment.apiUrl + "/vpns/" + name + "/domains/" + appDomainId + '/messages/' + messageName + '/publish';
    return this.http.get<MessageDefinitionAddedResult>(targetUrl);
  }
}
