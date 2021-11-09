import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let ApiDefService = class ApiDefService {
    constructor(http) {
        this.http = http;
    }
    add(name, path) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/apis";
        return this.http.post(targetUrl, { name: name, path: path }, { headers: headers });
    }
    addOperation(funcName, opName, opPath) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/apis/" + funcName + "/operations";
        return this.http.post(targetUrl, { opName: opName, opPath: opPath }, { headers: headers });
    }
    updateUIOperation(funcName, opName, ui) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/apis/" + funcName + "/operations/" + opName + "/ui";
        return this.http.put(targetUrl, { operationUI: ui }, { headers: headers });
    }
    search(startIndex, count, order, direction) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/apis/.search";
        return this.http.post(targetUrl, {
            startIndex: startIndex,
            count: count,
            orderBy: order,
            order: direction
        }, { headers: headers });
    }
    get(funcName) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/apis/" + funcName;
        return this.http.get(targetUrl, { headers: headers });
    }
    invokeOperation(funcName, opName, request) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/" + funcName;
        if (opName) {
            targetUrl += "/" + opName;
        }
        return this.http.post(targetUrl, request, { headers: headers });
    }
};
ApiDefService = __decorate([
    Injectable()
], ApiDefService);
export { ApiDefService };
//# sourceMappingURL=api.service.js.map