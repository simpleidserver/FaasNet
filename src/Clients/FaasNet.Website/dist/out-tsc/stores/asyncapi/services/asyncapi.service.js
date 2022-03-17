import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let AsyncApiService = class AsyncApiService {
    constructor(http) {
        this.http = http;
    }
    getOperations(endpoint) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + '/asyncapi/operations';
        return this.http.post(targetUrl, {
            endpoint: endpoint
        }, { headers: headers });
    }
    getOperation(endpoint, operationId) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + '/asyncapi/operations/' + operationId;
        return this.http.post(targetUrl, {
            endpoint: endpoint
        }, { headers: headers });
    }
};
AsyncApiService = __decorate([
    Injectable()
], AsyncApiService);
export { AsyncApiService };
//# sourceMappingURL=asyncapi.service.js.map