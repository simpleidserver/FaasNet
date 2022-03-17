import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let OpenApiService = class OpenApiService {
    constructor(http) {
        this.http = http;
    }
    getOperations(endpoint) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + '/openapi/operations';
        return this.http.post(targetUrl, {
            endpoint: endpoint
        }, { headers: headers });
    }
    getOperation(endpoint, operationId) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + '/openapi/operations/' + operationId;
        return this.http.post(targetUrl, {
            endpoint: endpoint
        }, { headers: headers });
    }
};
OpenApiService = __decorate([
    Injectable()
], OpenApiService);
export { OpenApiService };
//# sourceMappingURL=openapi.service.js.map