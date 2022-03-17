import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeAddBridge, completeGetAll, errorAdd, errorAddBridge, errorGetAll, startAdd, startAddBridge, startGetAll } from '../actions/eventmeshserver.actions';
let EventMeshServerEffects = class EventMeshServerEffects {
    constructor(actions$, eventMeshServerService) {
        this.actions$ = actions$;
        this.eventMeshServerService = eventMeshServerService;
        this.addEventMeshServer = this.actions$
            .pipe(ofType(startAdd), mergeMap((evt) => {
            return this.eventMeshServerService.add(evt.isLocalhost, evt.urn, evt.port)
                .pipe(map(content => completeAdd({ content: content })), catchError(() => of(errorAdd())));
        }));
        this.getAllEventMeshServers = this.actions$
            .pipe(ofType(startGetAll), mergeMap(() => {
            return this.eventMeshServerService.getAll()
                .pipe(map(content => completeGetAll({ content: content })), catchError(() => of(errorGetAll())));
        }));
        this.addEventMeshServerBridge = this.actions$
            .pipe(ofType(startAddBridge), mergeMap((evt) => {
            return this.eventMeshServerService.addBridge(evt.from, evt.fromPort, evt.to, evt.toPort)
                .pipe(map(content => completeAddBridge()), catchError(() => of(errorAddBridge())));
        }));
    }
};
__decorate([
    Effect()
], EventMeshServerEffects.prototype, "addEventMeshServer", void 0);
__decorate([
    Effect()
], EventMeshServerEffects.prototype, "getAllEventMeshServers", void 0);
__decorate([
    Effect()
], EventMeshServerEffects.prototype, "addEventMeshServerBridge", void 0);
EventMeshServerEffects = __decorate([
    Injectable()
], EventMeshServerEffects);
export { EventMeshServerEffects };
//# sourceMappingURL=eventmeshserver.effects.js.map