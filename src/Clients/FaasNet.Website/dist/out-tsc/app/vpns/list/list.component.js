import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { deleteVpn, startAddVpn, startGetAllVpn } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
import { AddVpnComponent } from './add-vpn.component';
let ListVpnComponent = class ListVpnComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['actions', 'name', 'description', 'createDateTime', 'updateDateTime'];
        this.isLoading = false;
        this.vpns = [];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_ADD_VPN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.vpnAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_DELETE_VPN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.vpnRemoved'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_DELETE_VPN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveVpn'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_ADD_VPN'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorAddVpn'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_GET_ALL'))
            .subscribe(() => {
            this.isLoading = false;
        });
        this.store.pipe(select(fromReducers.selectVpnLstResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.vpns = state;
            this.isLoading = false;
        });
        this.refresh();
    }
    addVpn() {
        const dialogRef = this.dialog.open(AddVpnComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            this.isLoading = true;
            const addVpn = startAddVpn({ name: e.name, description: e.description });
            this.store.dispatch(addVpn);
        });
    }
    removeVpn(vpn) {
        this.isLoading = true;
        const removeVpn = deleteVpn({ name: vpn.name });
        this.store.dispatch(removeVpn);
    }
    refresh() {
        this.isLoading = true;
        const getAllVpn = startGetAllVpn();
        this.store.dispatch(getAllVpn);
    }
};
ListVpnComponent = __decorate([
    Component({
        selector: 'list-applicationdomains',
        templateUrl: './list.component.html',
        styleUrls: ['./list.component.scss']
    })
], ListVpnComponent);
export { ListVpnComponent };
//# sourceMappingURL=list.component.js.map