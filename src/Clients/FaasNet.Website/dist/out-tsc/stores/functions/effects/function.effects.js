import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeDelete, completeGet, completeGetConfiguration, completeGetCpuUsage, completeGetDetails, completeGetRequestDuration, completeGetThreads, completeGetTotalRequests, completeGetVirtualMemoryBytes, completeInvoke, completeSearch, errorAdd, errorDelete, errorGet, errorGetConfiguration, errorGetCpuUsage, errorGetDetails, errorGetRequestDuration, errorGetThreads, errorGetTotalRequests, errorGetVirtualMemoryBytes, errorInvoke, errorSearch, startAdd, startDelete, startGet, startGetConfiguration, startGetCpuUsage, startGetDetails, startGetRequestDuration, startGetThreads, startGetTotalRequests, startGetVirtualMemoryBytes, startInvoke, startSearch } from '../actions/function.actions';
let FunctionEffects = class FunctionEffects {
    constructor(actions$, applicationService) {
        this.actions$ = actions$;
        this.applicationService = applicationService;
        this.searchFunctions$ = this.actions$
            .pipe(ofType(startSearch), mergeMap((evt) => {
            return this.applicationService.search(evt.startIndex, evt.count, evt.order, evt.direction)
                .pipe(map(content => completeSearch({ content: content })), catchError(() => of(errorSearch())));
        }));
        this.getFunctionConfiguration$ = this.actions$
            .pipe(ofType(startGetConfiguration), mergeMap((evt) => {
            return this.applicationService.getConfiguration(evt.name)
                .pipe(map(content => completeGetConfiguration({ content: content })), catchError(() => of(errorGetConfiguration())));
        }));
        this.invokeFunction$ = this.actions$
            .pipe(ofType(startInvoke), mergeMap((evt) => {
            return this.applicationService.invoke(evt.name, evt.request)
                .pipe(map(content => completeInvoke({ content: content })), catchError(() => of(errorInvoke())));
        }));
        this.getFunction$ = this.actions$
            .pipe(ofType(startGet), mergeMap((evt) => {
            return this.applicationService.get(evt.name)
                .pipe(map(content => completeGet({ content: content })), catchError(() => of(errorGet())));
        }));
        this.deleteFunction$ = this.actions$
            .pipe(ofType(startDelete), mergeMap((evt) => {
            return this.applicationService.delete(evt.name)
                .pipe(map(content => completeDelete()), catchError(() => of(errorDelete())));
        }));
        this.addFunction$ = this.actions$
            .pipe(ofType(startAdd), mergeMap((evt) => {
            return this.applicationService.add(evt.name, evt.description, evt.image, evt.version)
                .pipe(map(content => completeAdd({ name: evt.name, image: evt.image, description: evt.description, version: evt.version })), catchError(() => of(errorAdd())));
        }));
        this.getThreads$ = this.actions$
            .pipe(ofType(startGetThreads), mergeMap((evt) => {
            return this.applicationService.getThreads(evt.name, evt.startDate, evt.endDate)
                .pipe(map(content => completeGetThreads({ content: content })), catchError(() => of(errorGetThreads())));
        }));
        this.getVirtualMemoryBytes$ = this.actions$
            .pipe(ofType(startGetVirtualMemoryBytes), mergeMap((evt) => {
            return this.applicationService.getVirtualMemoryBytes(evt.name, evt.startDate, evt.endDate)
                .pipe(map(content => completeGetVirtualMemoryBytes({ content: content })), catchError(() => of(errorGetVirtualMemoryBytes())));
        }));
        this.getCpuUsage$ = this.actions$
            .pipe(ofType(startGetCpuUsage), mergeMap((evt) => {
            return this.applicationService.getCpuUsage(evt.name, evt.startDate, evt.endDate, evt.duration)
                .pipe(map(content => completeGetCpuUsage({ content: content })), catchError(() => of(errorGetCpuUsage())));
        }));
        this.getRequestDuration$ = this.actions$
            .pipe(ofType(startGetRequestDuration), mergeMap((evt) => {
            return this.applicationService.getRequestDuration(evt.name, evt.startDate, evt.endDate, evt.duration)
                .pipe(map(content => completeGetRequestDuration({ content: content })), catchError(() => of(errorGetRequestDuration())));
        }));
        this.getDetails$ = this.actions$
            .pipe(ofType(startGetDetails), mergeMap((evt) => {
            return this.applicationService.getDetails(evt.name)
                .pipe(map(content => completeGetDetails({ content: content })), catchError(() => of(errorGetDetails())));
        }));
        this.getTotalRequests$ = this.actions$
            .pipe(ofType(startGetTotalRequests), mergeMap((evt) => {
            return this.applicationService.getTotalRequests(evt.name, evt.time)
                .pipe(map(content => completeGetTotalRequests({ content: content })), catchError(() => of(errorGetTotalRequests())));
        }));
    }
};
__decorate([
    Effect()
], FunctionEffects.prototype, "searchFunctions$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getFunctionConfiguration$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "invokeFunction$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getFunction$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "deleteFunction$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "addFunction$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getThreads$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getVirtualMemoryBytes$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getCpuUsage$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getRequestDuration$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getDetails$", void 0);
__decorate([
    Effect()
], FunctionEffects.prototype, "getTotalRequests$", void 0);
FunctionEffects = __decorate([
    Injectable()
], FunctionEffects);
export { FunctionEffects };
//# sourceMappingURL=function.effects.js.map