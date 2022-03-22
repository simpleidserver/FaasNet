import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startAddAppDomain, startDeleteAppDomain, startGetAppDomains } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
import { AddAppDomainComponent } from './add-appdomain.component';
let AppDomainsVpnComponent = class AppDomainsVpnComponent {
    constructor(store, activatedRoute, actions$, snackBar, translateService, dialog) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.snackBar = snackBar;
        this.translateService = translateService;
        this.dialog = dialog;
        this.appDomains = [];
        this.displayedColumns = ['actions', 'name', 'description', 'rootTopic', 'createDateTime', 'updateDateTime'];
        this.name = "";
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_ADD_APPDOMAIN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.appDomainAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_ADD_APPDOMAIN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorAddAppDomain'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_DELETE_APPDOMAIN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.appDomainRemoved'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_DELETE_APPDOMAIN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveAppDomain'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectAppDomainsResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.appDomains = state;
        });
        this.refresh();
    }
    addAppDomain() {
        const dialogRef = this.dialog.open(AddAppDomainComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            const act = startAddAppDomain({ vpn: this.name, name: e.name, description: e.description, rootTopic: e.rootTopic });
            this.store.dispatch(act);
        });
    }
    removeAppDomain(appDomain) {
        const act = startDeleteAppDomain({ name: this.name, appDomainId: appDomain.id });
        this.store.dispatch(act);
    }
    refresh() {
        var _a;
        this.name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const act = startGetAppDomains({ name: this.name });
        this.store.dispatch(act);
    }
};
AppDomainsVpnComponent = __decorate([
    Component({
        selector: 'appdomains-vpn',
        templateUrl: './appdomains.component.html',
        styleUrls: ['./appdomains.component.scss']
    })
], AppDomainsVpnComponent);
export { AppDomainsVpnComponent };
//# sourceMappingURL=appdomains.component.js.map