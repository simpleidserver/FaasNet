import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddEmpty, completeGetJson, completeLaunch, completeSearch, completeUpdate, errorAddEmpty, errorGetJson, errorLaunch, errorSearch, errorUpdate, startAddEmpty, startGetJson, startLaunch, startSearch, startUpdate } from '../actions/statemachines.actions';
let StateMachineEffects = class StateMachineEffects {
    constructor(actions$, stateMachinesService) {
        this.actions$ = actions$;
        this.stateMachinesService = stateMachinesService;
        this.searchStateMachines = this.actions$
            .pipe(ofType(startSearch), mergeMap((evt) => {
            return this.stateMachinesService.search(evt.startIndex, evt.count, evt.order, evt.direction)
                .pipe(map(content => completeSearch({ content: content })), catchError(() => of(errorSearch())));
        }));
        this.getStateMachineJson = this.actions$
            .pipe(ofType(startGetJson), mergeMap((evt) => {
            return this.stateMachinesService.getJson(evt.id)
                .pipe(map(content => completeGetJson({ content: content })), catchError(() => of(errorGetJson())));
        }));
        this.addEmptyStateMachine = this.actions$
            .pipe(ofType(startAddEmpty), mergeMap((evt) => {
            return this.stateMachinesService.addEmpty(evt.name, evt.description)
                .pipe(map(content => completeAddEmpty({ id: content.id })), catchError(() => of(errorAddEmpty())));
        }));
        this.updateStateMachine = this.actions$
            .pipe(ofType(startUpdate), mergeMap((evt) => {
            return this.stateMachinesService.update(evt.id, evt.stateMachine)
                .pipe(map(content => completeUpdate({ id: content.id })), catchError(() => of(errorUpdate())));
        }));
        this.launchStateMachine = this.actions$
            .pipe(ofType(startLaunch), mergeMap((evt) => {
            return this.stateMachinesService.launch(evt.id, evt.input, evt.parameters)
                .pipe(map(content => completeLaunch({ id: content.id, launchDateTime: content.launchDateTime })), catchError(() => of(errorLaunch())));
        }));
    }
};
__decorate([
    Effect()
], StateMachineEffects.prototype, "searchStateMachines", void 0);
__decorate([
    Effect()
], StateMachineEffects.prototype, "getStateMachineJson", void 0);
__decorate([
    Effect()
], StateMachineEffects.prototype, "addEmptyStateMachine", void 0);
__decorate([
    Effect()
], StateMachineEffects.prototype, "updateStateMachine", void 0);
__decorate([
    Effect()
], StateMachineEffects.prototype, "launchStateMachine", void 0);
StateMachineEffects = __decorate([
    Injectable()
], StateMachineEffects);
export { StateMachineEffects };
//# sourceMappingURL=statemachines.effects.js.map