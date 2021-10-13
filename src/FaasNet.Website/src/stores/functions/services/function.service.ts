import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';
import { PrometheusQueryRangeResult, PrometheusQueryResult } from '../../common/prometheus-query.model';
import { SearchResult } from '../../common/search.model';
import { FunctionDetailsResult } from '../models/function-details.model';
import { FunctionResult } from '../models/function.model';

@Injectable()
export class FunctionService {
  constructor(
    private http: HttpClient) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<FunctionResult>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/.search";
    return this.http.post<SearchResult<FunctionResult>>(targetUrl, {
      startIndex: startIndex,
      count: count,
      orderBy: order,
      order: direction
    }, { headers: headers });
  }

  getConfiguration(name: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + '/configuration';
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  invoke(name: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + '/invoke';
    return this.http.post<any>(targetUrl, request, { headers: headers });
  }

  get(name: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name;
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  delete(name: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name;
    return this.http.delete<any>(targetUrl, { headers: headers });
  }

  add(name: string, image: string): Observable<FunctionResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions";
    return this.http.post<any>(targetUrl, { name: name, image: image }, { headers: headers });
  }

  getThreads(name: string, startDate: number, endDate: number): Observable<PrometheusQueryRangeResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=process_num_threads{job=%22"+name+"%22}&start="+startDate+"&end="+endDate+"&step=14";
    return this.http.get<PrometheusQueryRangeResult>(targetUrl, { headers: headers });
  }

  getVirtualMemoryBytes(name: string, startDate: number, endDate: number): Observable<PrometheusQueryRangeResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=process_virtual_memory_bytes{job=%22" + name + "%22}&start=" + startDate + "&end=" + endDate + "&step=14";
    return this.http.get<PrometheusQueryRangeResult>(targetUrl, { headers: headers });
  }

  getCpuUsage(name: string, startDate: number, endDate: number, duration: number): Observable<PrometheusQueryRangeResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=avg(rate(process_cpu_seconds_total{job='" + name + "'}[" + duration + "m]))&start=" + startDate + "&end=" + endDate + "&step=14";
    return this.http.get<PrometheusQueryRangeResult>(targetUrl, { headers: headers });
  }

  getRequestDuration(name: string, startDate: number, endDate: number, duration: number): Observable<PrometheusQueryRangeResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=avg(rate(request_duration_seconds_bucket{job='" + name + "'}[" + duration + "m]))&start=" + startDate + "&end=" + endDate + "&step=14";
    return this.http.get<PrometheusQueryRangeResult>(targetUrl, { headers: headers });
  }

  getDetails(name: string): Observable<FunctionDetailsResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/details";
    return this.http.get<FunctionDetailsResult>(targetUrl, { headers: headers });
  }

  getTotalRequests(name: string, startDate: number): Observable<PrometheusQueryResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring?query=total_requests&time=" + startDate;
    return this.http.get<PrometheusQueryResult>(targetUrl, { headers: headers });
  }
}
