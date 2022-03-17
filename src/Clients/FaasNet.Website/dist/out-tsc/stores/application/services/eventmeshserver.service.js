import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let ApplicationService = class ApplicationService {
    constructor(http) {
        this.http = http;
    }
    addApplicationDomain(rootTopic, name, description) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/applicationdomains";
        return this.http.post(targetUrl, { rootTopic: rootTopic, name: name, description: description }, { headers: headers });
    }
    getAllApplicationDomains() {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/applicationdomains";
        return this.http.get(targetUrl, { headers: headers });
    }
};
ApplicationService = __decorate([
    Injectable()
], ApplicationService);
export { ApplicationService };
//# sourceMappingURL=eventmeshserver.service.js.map