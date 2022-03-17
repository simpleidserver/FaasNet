import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let EventMeshServerService = class EventMeshServerService {
    constructor(http) {
        this.http = http;
    }
    add(isLocalhost, urn, port) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/eventmesh";
        return this.http.post(targetUrl, { isLocalhost: isLocalhost, urn: urn, port: port }, { headers: headers });
    }
    getAll() {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/eventmesh";
        return this.http.get(targetUrl, { headers: headers });
    }
    addBridge(from, fromPort, to, toPort) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/eventmesh/bridge";
        return this.http.post(targetUrl, { from: { urn: from, port: fromPort }, to: { urn: to, port: toPort } }, { headers: headers });
    }
};
EventMeshServerService = __decorate([
    Injectable()
], EventMeshServerService);
export { EventMeshServerService };
//# sourceMappingURL=eventmeshserver.service.js.map