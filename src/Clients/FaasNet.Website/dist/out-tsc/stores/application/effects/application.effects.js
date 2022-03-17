import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddApplicationDomain, completeGetAllApplicationDomains, errorAddApplicationDomain, errorGetAllApplicationDomains, startAddApplicationDomain, startGetAllApplicationDomains } from '../actions/application.actions';
let ApplicationEffects = class ApplicationEffects {
    constructor(actions$, applicationService) {
        this.actions$ = actions$;
        this.applicationService = applicationService;
        this.addEventMeshServer = this.actions$
            .pipe(ofType(startAddApplicationDomain), mergeMap((evt) => {
            return this.applicationService.addApplicationDomain(evt.rootTopic, evt.name, evt.description)
                .pipe(map(content => completeAddApplicationDomain({ content: content })), catchError(() => of(errorAddApplicationDomain())));
        }));
        this.getAllApplicationDomains = this.actions$
            .pipe(ofType(startGetAllApplicationDomains), mergeMap(() => {
            return this.applicationService.getAllApplicationDomains()
                .pipe(map(content => completeGetAllApplicationDomains({ content: content })), catchError(() => of(errorGetAllApplicationDomains())));
        }));
    }
};
__decorate([
    Effect()
], ApplicationEffects.prototype, "addEventMeshServer", void 0);
__decorate([
    Effect()
], ApplicationEffects.prototype, "getAllApplicationDomains", void 0);
ApplicationEffects = __decorate([
    Injectable()
], ApplicationEffects);
export { ApplicationEffects };
//# sourceMappingURL=application.effects.js.map