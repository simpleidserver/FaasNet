import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { ServerStatusResult } from '../models/serverstatus.model';

@Injectable()
export class ServerService {
  constructor(
    private http: HttpClient) { }

  getStatus(): Observable<ServerStatusResult> {
    let targetUrl = environment.apiUrl + "/server/status";
    return this.http.get<ServerStatusResult>(targetUrl);
  }
}
