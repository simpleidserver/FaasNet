import { __decorate } from "tslib";
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let StateMachineInstancesService = class StateMachineInstancesService {
    constructor(http) {
        this.http = http;
    }
    search(startIndex, count, order, direction) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachineinstances/.search";
        return this.http.post(targetUrl, {
            startIndex: startIndex,
            count: count,
            orderBy: order,
            order: direction
        }, { headers: headers });
    }
    get(id) {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        let targetUrl = environment.apiUrl + "/statemachineinstances/" + id;
        return this.http.get(targetUrl, { headers: headers });
    }
};
StateMachineInstancesService = __decorate([
    Injectable()
], StateMachineInstancesService);
export { StateMachineInstancesService };
//# sourceMappingURL=statemachineinstances.service.js.map