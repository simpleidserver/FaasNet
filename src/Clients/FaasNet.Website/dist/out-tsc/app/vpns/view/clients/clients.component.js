import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startAddClient, startDeleteClient, startGetAllClients } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
import { AddClientComponent } from './add-client.component';
let ClientsVpnComponent = class ClientsVpnComponent {
    constructor(store, activatedRoute, actions$, snackBar, translateService, dialog) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.snackBar = snackBar;
        this.translateService = translateService;
        this.dialog = dialog;
        this.clients = [];
        this.displayedColumns = ['actions', 'clientId', 'purposes', 'createDateTime'];
        this.name = "";
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_ADD_CLIENT'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.clientAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_ADD_CLIENT'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorAddClient'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_DELETE_CLIENT'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.clientRemoved'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_DELETE_CLIENT'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveClient'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectClientsResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.clients = state;
        });
        this.refresh();
    }
    getPurpose(purpose) {
        switch (purpose) {
            case 1:
                return 'Subscribe';
            case 2:
                return 'Publish';
        }
        return '';
    }
    addClient() {
        const dialogRef = this.dialog.open(AddClientComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            const purposes = e.purposes.map((p) => parseInt(p));
            const act = startAddClient({ name: this.name, clientId: e.clientId, purposes: purposes });
            this.store.dispatch(act);
        });
    }
    removeClient(client) {
        const act = startDeleteClient({ clientId: client.clientId, name: this.name });
        this.store.dispatch(act);
    }
    refresh() {
        var _a;
        this.name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const act = startGetAllClients({ name: this.name });
        this.store.dispatch(act);
    }
};
ClientsVpnComponent = __decorate([
    Component({
        selector: 'clients-vpn',
        templateUrl: './clients.component.html',
        styleUrls: ['./clients.component.scss']
    })
], ClientsVpnComponent);
export { ClientsVpnComponent };
//# sourceMappingURL=clients.component.js.map