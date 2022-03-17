import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { ServerStatusResult } from '@stores/server/models/serverstatus.model';
import { startGetAllVpn } from '@stores/vpn/actions/vpn.actions';
import { startGetServerStatus } from '../stores/server/actions/server.actions';
let AppComponent = class AppComponent {
    constructor(translate, store) {
        this.translate = translate;
        this.store = store;
        this.vpnLstResult = [];
        this.serverStatus = new ServerStatusResult();
        this.vpnFormControl = new FormControl();
        this.translate.setDefaultLang('en');
        this.translate.use('en');
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectServerStatusResult)).subscribe((serverStatus) => {
            if (!serverStatus || serverStatus.isRunning === false) {
                return;
            }
            this.serverStatus = serverStatus;
            const getAllVpn = startGetAllVpn();
            this.store.dispatch(getAllVpn);
        });
        this.store.pipe(select(fromReducers.selectVpnLstResult)).subscribe((vpnLstResult) => {
            if (!vpnLstResult) {
                return;
            }
            this.vpnLstResult = vpnLstResult;
        });
        this.vpnFormControl.valueChanges.subscribe(() => {
        });
        const getServerStatus = startGetServerStatus();
        this.store.dispatch(getServerStatus);
    }
    ngAfterViewInit() {
    }
};
AppComponent = __decorate([
    Component({
        selector: 'app-root',
        templateUrl: './app.component.html',
        styleUrls: ['./app.component.scss']
    })
], AppComponent);
export { AppComponent };
//# sourceMappingURL=app.component.js.map