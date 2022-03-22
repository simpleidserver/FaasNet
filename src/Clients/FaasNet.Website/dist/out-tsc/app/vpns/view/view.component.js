import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { startGetVpn } from '@stores/vpn/actions/vpn.actions';
let ViewVpnComponent = class ViewVpnComponent {
    constructor(store, activatedRoute) {
        this.store = store;
        this.activatedRoute = activatedRoute;
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
        this.name = this.activatedRoute.snapshot.params['name'];
        if (this.name) {
            const action = startGetVpn({ name: this.name });
            this.store.dispatch(action);
        }
    }
};
ViewVpnComponent = __decorate([
    Component({
        selector: 'view-vpn',
        templateUrl: './view.component.html'
    })
], ViewVpnComponent);
export { ViewVpnComponent };
//# sourceMappingURL=view.component.js.map