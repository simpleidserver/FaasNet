import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
let ViewVpnClientSessionsComponent = class ViewVpnClientSessionsComponent {
    constructor(store) {
        this.store = store;
        this.sessions = [];
        this.displayedColumns = ['purpose', 'bufferCloudEvents', 'topics', 'state', 'createDateTime'];
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectClientResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.sessions = state.sessions;
        });
    }
    ngOnDestroy() {
    }
};
ViewVpnClientSessionsComponent = __decorate([
    Component({
        selector: 'view-vpn-client-sessions',
        templateUrl: './sessions.component.html'
    })
], ViewVpnClientSessionsComponent);
export { ViewVpnClientSessionsComponent };
//# sourceMappingURL=sessions.component.js.map