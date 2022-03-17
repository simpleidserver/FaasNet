import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddVpn, completeDeleteVpn, completeGetAllVpn, deleteVpn, errorAddVpn, errorGetAllVpn, startAddVpn, startGetAllVpn } from '../actions/vpn.actions';
let VpnEffects = class VpnEffects {
    constructor(actions$, vpnService) {
        this.actions$ = actions$;
        this.vpnService = vpnService;
        this.getAllVpn = this.actions$
            .pipe(ofType(startGetAllVpn), mergeMap(() => {
            return this.vpnService.getAllVpn()
                .pipe(map(content => completeGetAllVpn({ content: content })), catchError(() => of(errorGetAllVpn())));
        }));
        this.addVpn = this.actions$
            .pipe(ofType(startAddVpn), mergeMap((evt) => {
            return this.vpnService.addVpn(evt.name, evt.description)
                .pipe(map(content => completeAddVpn(evt)), catchError(() => of(errorAddVpn())));
        }));
        this.deleteVpn = this.actions$
            .pipe(ofType(deleteVpn), mergeMap((evt) => {
            return this.vpnService.deleteVpn(evt.name)
                .pipe(map(content => completeDeleteVpn(evt)), catchError(() => of(errorAddVpn())));
        }));
    }
};
__decorate([
    Effect()
], VpnEffects.prototype, "getAllVpn", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "addVpn", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "deleteVpn", void 0);
VpnEffects = __decorate([
    Injectable()
], VpnEffects);
export { VpnEffects };
//# sourceMappingURL=vpn.effects.js.map