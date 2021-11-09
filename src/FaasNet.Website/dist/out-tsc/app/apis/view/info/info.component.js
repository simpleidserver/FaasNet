import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import { startGet } from '@stores/apis/actions/api.actions';
import * as fromReducers from '@stores/appstate';
let InfoApiComponent = class InfoApiComponent {
    constructor(store, activatedRoute) {
        this.store = store;
        this.activatedRoute = activatedRoute;
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.api = state;
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
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        const action = startGet({ funcName: name });
        this.store.dispatch(action);
    }
};
InfoApiComponent = __decorate([
    Component({
        selector: 'info-function',
        templateUrl: './info.component.html'
    })
], InfoApiComponent);
export { InfoApiComponent };
//# sourceMappingURL=info.component.js.map