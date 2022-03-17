import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGet } from '@stores/functions/actions/function.actions';
let ViewFunctionComponent = class ViewFunctionComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.name = state.name;
            this.id = state.id;
        });
        this.subscription = this.activatedRoute.params.subscribe(() => {
            this.refresh();
        });
    }
    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
    refresh() {
        const name = this.activatedRoute.snapshot.params['name'];
        const action = startGet({ name: name });
        this.store.dispatch(action);
    }
};
ViewFunctionComponent = __decorate([
    Component({
        selector: 'view-function',
        templateUrl: './view.component.html'
    })
], ViewFunctionComponent);
export { ViewFunctionComponent };
//# sourceMappingURL=view.component.js.map