import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetAppDomain } from '@stores/vpn/actions/vpn.actions';
let ViewVpnAppDomainComponent = class ViewVpnAppDomainComponent {
    constructor(store, activatedRoute) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.vpnName = "";
        this.appDomainId = "";
        this.appDomainName = "";
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectAppDomainResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.appDomainName = state.name;
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
        const params = this.activatedRoute.snapshot.params;
        this.vpnName = params['vpnName'];
        this.appDomainId = params['appDomainId'];
        const act = startGetAppDomain({ name: this.vpnName, appDomainId: this.appDomainId });
        this.store.dispatch(act);
    }
};
ViewVpnAppDomainComponent = __decorate([
    Component({
        selector: 'view-vpn-appdomain',
        templateUrl: './view.component.html'
    })
], ViewVpnAppDomainComponent);
export { ViewVpnAppDomainComponent };
//# sourceMappingURL=view.component.js.map