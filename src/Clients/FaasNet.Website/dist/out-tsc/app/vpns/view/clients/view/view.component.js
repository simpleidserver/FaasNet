import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { startGetClient } from '@stores/vpn/actions/vpn.actions';
let ViewVpnClientComponent = class ViewVpnClientComponent {
    constructor(store, activatedRoute) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.vpnName = "";
        this.clientId = "";
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
        const params = this.activatedRoute.snapshot.params;
        this.vpnName = params['vpnName'];
        this.clientId = params['clientId'];
        const act = startGetClient({ name: this.vpnName, clientId: this.clientId });
        this.store.dispatch(act);
    }
};
ViewVpnClientComponent = __decorate([
    Component({
        selector: 'view-vpn-client',
        templateUrl: './view.component.html'
    })
], ViewVpnClientComponent);
export { ViewVpnClientComponent };
//# sourceMappingURL=view.component.js.map