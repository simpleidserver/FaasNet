import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let FunctionService = class FunctionService {
    constructor(http) {
        this.http = http;
    }
    search(startIndex, count, order, direction) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/.search";
        return this.http.post(targetUrl, {
            startIndex: startIndex,
            count: count,
            orderBy: order,
            order: direction
        }, { headers: headers });
    }
    getConfiguration(name) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + '/configuration';
        return this.http.get(targetUrl, { headers: headers });
    }
    invoke(name, request) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + '/invoke';
        return this.http.post(targetUrl, request, { headers: headers });
    }
    get(name) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name;
        return this.http.get(targetUrl, { headers: headers });
    }
    delete(name) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name;
        return this.http.delete(targetUrl, { headers: headers });
    }
    add(name, image) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions";
        return this.http.post(targetUrl, { name: name, image: image }, { headers: headers });
    }
    getThreads(name, startDate, endDate) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=process_num_threads{job=%22" + name + "%22}&start=" + startDate + "&end=" + endDate + "&step=14";
        return this.http.get(targetUrl, { headers: headers });
    }
    getVirtualMemoryBytes(name, startDate, endDate) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=process_virtual_memory_bytes{job=%22" + name + "%22}&start=" + startDate + "&end=" + endDate + "&step=14";
        return this.http.get(targetUrl, { headers: headers });
    }
    getCpuUsage(name, startDate, endDate, duration) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=avg(rate(process_cpu_seconds_total{job='" + name + "'}[" + duration + "m]))&start=" + startDate + "&end=" + endDate + "&step=14";
        return this.http.get(targetUrl, { headers: headers });
    }
    getRequestDuration(name, startDate, endDate, duration) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring/range?query=avg(rate(request_duration_seconds_bucket{job='" + name + "'}[" + duration + "m]))&start=" + startDate + "&end=" + endDate + "&step=14";
        return this.http.get(targetUrl, { headers: headers });
    }
    getDetails(name) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/details";
        return this.http.get(targetUrl, { headers: headers });
    }
    getTotalRequests(name, startDate) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/functions/" + name + "/monitoring?query=total_requests&time=" + startDate;
        return this.http.get(targetUrl, { headers: headers });
    }
};
FunctionService = __decorate([
    Injectable()
], FunctionService);
export { FunctionService };
//# sourceMappingURL=function.service.js.map