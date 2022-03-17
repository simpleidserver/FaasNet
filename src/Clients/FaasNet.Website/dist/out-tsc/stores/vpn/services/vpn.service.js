import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let VpnService = class VpnService {
    constructor(http) {
        this.http = http;
    }
    getAllVpn() {
        let targetUrl = environment.apiUrl + "/vpns";
        return this.http.get(targetUrl);
    }
    addVpn(name, description) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/vpns";
        return this.http.post(targetUrl, {
            name: name,
            description: description
        }, { headers: headers });
    }
    addApplicationDomain(vpn, rootTopic, name, description) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/domains';
        const json = {
            rootTopic: rootTopic,
            name: name,
            description: description
        };
        return this.http.post(targetUrl, json, { headers: headers });
    }
    deleteApplicationDomain(vpn, applicationDomainId) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/domains/' + applicationDomainId;
        return this.http.delete(targetUrl);
    }
    addBridge(vpn, urn, port, targetVpn) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + '/bridges';
        const json = {
            urn: urn,
            port: port,
            targetVpn: targetVpn
        };
        return this.http.post(targetUrl, json, { headers: headers });
    }
    deleteVpn(vpn) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn;
        return this.http.delete(targetUrl);
    }
};
VpnService = __decorate([
    Injectable()
], VpnService);
export { VpnService };
//# sourceMappingURL=vpn.service.js.map