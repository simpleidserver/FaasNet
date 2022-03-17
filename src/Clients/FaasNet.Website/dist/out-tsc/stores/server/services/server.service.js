import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
let ServerService = class ServerService {
    constructor(http) {
        this.http = http;
    }
    getStatus() {
        let targetUrl = environment.apiUrl + "/server/status";
        return this.http.get(targetUrl);
    }
};
ServerService = __decorate([
    Injectable()
], ServerService);
export { ServerService };
//# sourceMappingURL=server.service.js.map