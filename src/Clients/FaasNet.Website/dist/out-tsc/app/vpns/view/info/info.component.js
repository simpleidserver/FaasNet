import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { deleteVpn } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
let InfoVpnComponent = class InfoVpnComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar, router) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.router = router;
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_DELETE_VPN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.vpnRemoved'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.router.navigate(['/vpns']);
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_DELETE_VPN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveVpn'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectVpnResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.vpn = state;
        });
    }
    ngOnDestroy() {
    }
    delete() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = deleteVpn({ name: name });
        this.store.dispatch(action);
    }
};
InfoVpnComponent = __decorate([
    Component({
        selector: 'info-vpn',
        templateUrl: './info.component.html'
    })
], InfoVpnComponent);
export { InfoVpnComponent };
//# sourceMappingURL=info.component.js.map