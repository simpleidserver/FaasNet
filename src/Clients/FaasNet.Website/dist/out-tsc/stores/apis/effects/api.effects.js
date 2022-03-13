import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeGet, completeSearch, completeUpdateUIOperation, errorAdd, errorGet, errorAddOperation, errorSearch, errorUpdateUIOperation, startAdd, startAddOperation, startGet, startSearch, startUpdateUIOperation, completeAddOperation, startInvokeOperation, completeInvokeOperation, errorInvokeOperation } from '../actions/api.actions';
let ApiDefEffects = class ApiDefEffects {
    constructor(actions$, apiDefService) {
        this.actions$ = actions$;
        this.apiDefService = apiDefService;
        this.addApiDef$ = this.actions$
            .pipe(ofType(startAdd), mergeMap((evt) => {
            return this.apiDefService.add(evt.name, evt.path)
                .pipe(map(content => completeAdd()), catchError(() => of(errorAdd())));
        }));
        this.addApiDefOperation$ = this.actions$
            .pipe(ofType(startAddOperation), mergeMap((evt) => {
            return this.apiDefService.addOperation(evt.funcName, evt.opName, evt.opPath)
                .pipe(map(content => completeAddOperation()), catchError(() => of(errorAddOperation())));
        }));
        this.updateApiDefUI$ = this.actions$
            .pipe(ofType(startUpdateUIOperation), mergeMap((evt) => {
            return this.apiDefService.updateUIOperation(evt.funcName, evt.operationName, evt.ui)
                .pipe(map(content => completeUpdateUIOperation()), catchError(() => of(errorUpdateUIOperation())));
        }));
        this.searchApiDefs$ = this.actions$
            .pipe(ofType(startSearch), mergeMap((evt) => {
            return this.apiDefService.search(evt.startIndex, evt.count, evt.order, evt.direction)
                .pipe(map(content => completeSearch({ content: content })), catchError(() => of(errorSearch())));
        }));
        this.getApiDef = this.actions$
            .pipe(ofType(startGet), mergeMap((evt) => {
            return this.apiDefService.get(evt.funcName)
                .pipe(map(content => completeGet({ content: content })), catchError(() => of(errorGet())));
        }));
        this.invokeOperation = this.actions$
            .pipe(ofType(startInvokeOperation), mergeMap((evt) => {
            return this.apiDefService.invokeOperation(evt.funcName, evt.opName, evt.request)
                .pipe(map(content => completeInvokeOperation({ content: content })), catchError(() => of(errorInvokeOperation())));
        }));
    }
};
__decorate([
    Effect()
], ApiDefEffects.prototype, "addApiDef$", void 0);
__decorate([
    Effect()
], ApiDefEffects.prototype, "addApiDefOperation$", void 0);
__decorate([
    Effect()
], ApiDefEffects.prototype, "updateApiDefUI$", void 0);
__decorate([
    Effect()
], ApiDefEffects.prototype, "searchApiDefs$", void 0);
__decorate([
    Effect()
], ApiDefEffects.prototype, "getApiDef", void 0);
__decorate([
    Effect()
], ApiDefEffects.prototype, "invokeOperation", void 0);
ApiDefEffects = __decorate([
    Injectable()
], ApiDefEffects);
export { ApiDefEffects };
//# sourceMappingURL=api.effects.js.map