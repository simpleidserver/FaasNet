import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { addMessageDefinition, getLatestMessages, publishMessageDefinition, updateMessageDefinition } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
import { AddMessageDefComponent } from './add-message.component';
let MessagesVpnComponent = class MessagesVpnComponent {
    constructor(store, activatedRoute, dialog, actions$, snackBar, translateService) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.snackBar = snackBar;
        this.translateService = translateService;
        this.messages = [];
        this.vpnName = "";
        this.appDomainId = "";
        this.displayedColumns = ['actions', 'name', 'description', 'version', 'createDateTime', 'updateDateTime'];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_UPDATE_MESSAGE_DEFINITION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.messagDefUpdated'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_UPDATE_MESSAGE_DEFINITION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorUpdateMessageDef'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] COMPLETE_PUBLISH_MESSAGE_DEFINITION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.messageDefPublished'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.actions$.pipe(filter((action) => action.type === '[VPN] ERROR_PUBLISH_MESSAGE_DEFINITION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('vpn.messages.errorPublishMessage'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.messages = state;
        });
        this.refresh();
    }
    publishMessage(message) {
        const act = publishMessageDefinition({ vpn: this.vpnName, applicationDomainId: this.appDomainId, messageName: message.name });
        this.store.dispatch(act);
    }
    editMessage(message) {
        const dialogRef = this.dialog.open(AddMessageDefComponent, {
            width: '800px',
            data: message
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            const act = updateMessageDefinition({ vpn: this.vpnName, applicationDomainId: this.appDomainId, description: e.description, jsonSchema: e.jsonSchema, messageDefId: message.id });
            this.store.dispatch(act);
        });
    }
    addMessage() {
        const dialogRef = this.dialog.open(AddMessageDefComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            const act = addMessageDefinition({ applicationDomainId: this.appDomainId, description: e.description, name: e.name, vpn: this.vpnName, jsonSchema: e.jsonSchema });
            this.store.dispatch(act);
        });
    }
    refresh() {
        var _a, _b;
        this.vpnName = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['vpnName'];
        this.appDomainId = (_b = this.activatedRoute.parent) === null || _b === void 0 ? void 0 : _b.snapshot.params['appDomainId'];
        const act = getLatestMessages({ name: this.vpnName, appDomainId: this.appDomainId });
        this.store.dispatch(act);
    }
};
MessagesVpnComponent = __decorate([
    Component({
        selector: 'messages-vpn',
        templateUrl: './messages.component.html',
        styleUrls: ['./messages.component.scss']
    })
], MessagesVpnComponent);
export { MessagesVpnComponent };
//# sourceMappingURL=messages.component.js.map