import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGet, completeSearch, errorGet, errorSearch, startGet, startSearch } from '../actions/statemachineinstances.actions';
let StateMachineInstancesEffects = class StateMachineInstancesEffects {
    constructor(actions$, stateMachineInstancesService) {
        this.actions$ = actions$;
        this.stateMachineInstancesService = stateMachineInstancesService;
        this.searchStateMachineInstances$ = this.actions$
            .pipe(ofType(startSearch), mergeMap((evt) => {
            return this.stateMachineInstancesService.search(evt.startIndex, evt.count, evt.order, evt.direction)
                .pipe(map(content => completeSearch({ content: content })), catchError(() => of(errorSearch())));
        }));
        this.getStateMachineInstance$ = this.actions$
            .pipe(ofType(startGet), mergeMap((evt) => {
            return this.stateMachineInstancesService.get(evt.id)
                .pipe(map(content => completeGet({ content: content })), catchError(() => of(errorGet())));
        }));
    }
};
__decorate([
    Effect()
], StateMachineInstancesEffects.prototype, "searchStateMachineInstances$", void 0);
__decorate([
    Effect()
], StateMachineInstancesEffects.prototype, "getStateMachineInstance$", void 0);
StateMachineInstancesEffects = __decorate([
    Injectable()
], StateMachineInstancesEffects);
export { StateMachineInstancesEffects };
//# sourceMappingURL=statemachineinstances.effects.js.map