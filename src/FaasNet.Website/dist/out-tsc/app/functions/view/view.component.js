import { __decorate } from "tslib";
import { Component } from '@angular/core';
let ViewFunctionComponent = class ViewFunctionComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
    }
    ngOnInit() {
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
        this.name = name;
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