import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Document } from 'yaml';
let StateMachinesService = class StateMachinesService {
    constructor(http) {
        this.http = http;
    }
    search(startIndex, count, order, direction) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachines/.search";
        return this.http.post(targetUrl, {
            startIndex: startIndex,
            count: count,
            orderBy: order,
            order: direction
        }, { headers: headers });
    }
    getJson(id) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachines/" + id;
        return this.http.get(targetUrl);
    }
    addEmpty(name, description) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachines/empty";
        return this.http.post(targetUrl, {
            name: name,
            description: description
        }, { headers: headers });
    }
    update(id, stateMachine) {
        let headers = new HttpHeaders();
        headers = headers.set('Content-Type', 'text/yaml');
        let targetUrl = environment.apiUrl + "/statemachines/" + id;
        const json = {
            workflowDefinition: stateMachine
        };
        const doc = new Document();
        doc.contents = json;
        const yaml = doc.toString();
        return this.http.put(targetUrl, yaml, { headers: headers });
    }
    launch(id, input, parameters) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachines/start";
        return this.http.post(targetUrl, {
            id: id,
            input: input,
            parameters: parameters
        }, { headers: headers });
    }
};
StateMachinesService = __decorate([
    Injectable()
], StateMachinesService);
export { StateMachinesService };
//# sourceMappingURL=statemachines.service.js.map