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
    getVpn(vpn) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn;
        return this.http.get(targetUrl);
    }
    getAllClients(vpn) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients";
        return this.http.get(targetUrl);
    }
    getClient(vpn, clientId) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients/" + clientId;
        return this.http.get(targetUrl);
    }
    deleteClient(vpn, clientId) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients/" + clientId;
        return this.http.delete(targetUrl);
    }
    addClient(vpn, clientId, purposes) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/clients";
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        const json = {
            vpn: vpn,
            clientId: clientId,
            purposes: purposes
        };
        return this.http.post(targetUrl, json, { headers: headers });
    }
    addAppDomain(vpn, name, description, rootTopic) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains";
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        const json = {
            name: name,
            description: description,
            rootTopic: rootTopic
        };
        return this.http.post(targetUrl, json, { headers: headers });
    }
    getAppDomains(vpn) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains";
        return this.http.get(targetUrl);
    }
    getAppDomain(vpn, appDomainId) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + appDomainId;
        return this.http.get(targetUrl);
    }
    deleteAppDomain(vpn, appDomainId) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + appDomainId;
        return this.http.delete(targetUrl);
    }
    getLatestMessagesDef(name, appDomainId) {
        let targetUrl = environment.apiUrl + "/vpns/" + name + "/domains/" + appDomainId + '/messages/latest';
        return this.http.get(targetUrl);
    }
    addMessageDef(vpn, applicationDomainId, name, description, jsonSchema) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + applicationDomainId + '/messages';
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        const json = {
            name: name,
            description: description,
            jsonSchema: jsonSchema
        };
        return this.http.post(targetUrl, json, { headers: headers });
    }
    updateMessageDef(vpn, applicationDomainId, messageId, description, jsonSchema) {
        let targetUrl = environment.apiUrl + "/vpns/" + vpn + "/domains/" + applicationDomainId + '/messages/' + messageId;
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        const json = {
            description: description,
            jsonSchema: jsonSchema
        };
        return this.http.put(targetUrl, json, { headers: headers });
    }
    publishMessageDef(name, appDomainId, messageName) {
        let targetUrl = environment.apiUrl + "/vpns/" + name + "/domains/" + appDomainId + '/messages/' + messageName + '/publish';
        return this.http.get(targetUrl);
    }
};
VpnService = __decorate([
    Injectable()
], VpnService);
export { VpnService };
//# sourceMappingURL=vpn.service.js.map