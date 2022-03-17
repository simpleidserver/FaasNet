import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGetServerStatus, errorGetServerStatus, startGetServerStatus } from '../actions/server.actions';
let ServerEffects = class ServerEffects {
    constructor(actions$, serverService) {
        this.actions$ = actions$;
        this.serverService = serverService;
        this.getStatus = this.actions$
            .pipe(ofType(startGetServerStatus), mergeMap(() => {
            return this.serverService.getStatus()
                .pipe(map(content => completeGetServerStatus({ content: content })), catchError(() => of(errorGetServerStatus())));
        }));
    }
};
__decorate([
    Effect()
], ServerEffects.prototype, "getStatus", void 0);
ServerEffects = __decorate([
    Injectable()
], ServerEffects);
export { ServerEffects };
//# sourceMappingURL=server.effects.js.map