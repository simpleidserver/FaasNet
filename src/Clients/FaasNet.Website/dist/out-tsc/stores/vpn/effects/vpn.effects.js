import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
import { Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { addMessageDefinition, completeAddAppDomain, completeAddClient, completeAddMessageDefinition, completeAddVpn, completeDeleteAppDomain, completeDeleteClient, completeDeleteVpn, completeGetAllClients, completeGetAllVpn, completeGetAppDomain, completeGetAppDomains, completeGetClient, completeGetLatestMessages, completeGetVpn, completePublishMessageDefinition, completeUpdateMessageDefinition, deleteVpn, errorAddAppDomain, errorAddClient, errorAddMessageDefinition, errorAddVpn, errorDeleteAppDomain, errorDeleteClient, errorGetAllClients, errorGetAllVpn, errorGetAppDomain, errorGetAppDomains, errorGetClient, errorGetLatestMessages, errorGetVpn, errorPublishMessageDefinition, errorUpdateMessageDefinition, getLatestMessages, publishMessageDefinition, startAddAppDomain, startAddClient, startAddVpn, startDeleteAppDomain, startDeleteClient, startGetAllClients, startGetAllVpn, startGetAppDomain, startGetAppDomains, startGetClient, startGetVpn, updateMessageDefinition } from '../actions/vpn.actions';
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
        this.getVpn = this.actions$
            .pipe(ofType(startGetVpn), mergeMap((evt) => {
            return this.vpnService.getVpn(evt.name)
                .pipe(map(content => completeGetVpn({ content: content })), catchError(() => of(errorGetVpn())));
        }));
        this.getAllClients = this.actions$
            .pipe(ofType(startGetAllClients), mergeMap((evt) => {
            return this.vpnService.getAllClients(evt.name)
                .pipe(map(content => completeGetAllClients({ content: content })), catchError(() => of(errorGetAllClients())));
        }));
        this.getClient = this.actions$
            .pipe(ofType(startGetClient), mergeMap((evt) => {
            return this.vpnService.getClient(evt.name, evt.clientId)
                .pipe(map(content => completeGetClient({ content: content })), catchError(() => of(errorGetClient())));
        }));
        this.deleteClient = this.actions$
            .pipe(ofType(startDeleteClient), mergeMap((evt) => {
            return this.vpnService.deleteClient(evt.name, evt.clientId)
                .pipe(map(content => completeDeleteClient(evt)), catchError(() => of(errorDeleteClient())));
        }));
        this.addClient = this.actions$
            .pipe(ofType(startAddClient), mergeMap((evt) => {
            return this.vpnService.addClient(evt.name, evt.clientId, evt.purposes)
                .pipe(map(content => completeAddClient(evt)), catchError(() => of(errorAddClient())));
        }));
        this.addAppDomain = this.actions$
            .pipe(ofType(startAddAppDomain), mergeMap((evt) => {
            return this.vpnService.addAppDomain(evt.vpn, evt.name, evt.description, evt.rootTopic)
                .pipe(map(content => completeAddAppDomain({ id: content.id, name: evt.name, description: evt.description, rootTopic: evt.rootTopic })), catchError(() => of(errorAddAppDomain())));
        }));
        this.getAppDomains = this.actions$
            .pipe(ofType(startGetAppDomains), mergeMap((evt) => {
            return this.vpnService.getAppDomains(evt.name)
                .pipe(map(content => completeGetAppDomains({ content: content })), catchError(() => of(errorGetAppDomains())));
        }));
        this.getAppDomain = this.actions$
            .pipe(ofType(startGetAppDomain), mergeMap((evt) => {
            return this.vpnService.getAppDomain(evt.name, evt.appDomainId)
                .pipe(map(content => completeGetAppDomain({ content: content })), catchError(() => of(errorGetAppDomain())));
        }));
        this.deleteAppDomain = this.actions$
            .pipe(ofType(startDeleteAppDomain), mergeMap((evt) => {
            return this.vpnService.deleteAppDomain(evt.name, evt.appDomainId)
                .pipe(map(content => completeDeleteAppDomain({ name: evt.name, appDomainId: evt.appDomainId })), catchError(() => of(errorDeleteAppDomain())));
        }));
        this.getLatestMessages = this.actions$
            .pipe(ofType(getLatestMessages), mergeMap((evt) => {
            return this.vpnService.getLatestMessagesDef(evt.name, evt.appDomainId)
                .pipe(map(content => completeGetLatestMessages({ content: content })), catchError(() => of(errorGetLatestMessages())));
        }));
        this.addMsgDef = this.actions$
            .pipe(ofType(addMessageDefinition), mergeMap((evt) => {
            return this.vpnService.addMessageDef(evt.vpn, evt.applicationDomainId, evt.name, evt.description, evt.jsonSchema)
                .pipe(map(content => completeAddMessageDefinition({ vpn: evt.vpn, messageDefId: content.id, applicationDomainId: evt.applicationDomainId, description: evt.description, jsonSchema: evt.jsonSchema, name: evt.name })), catchError(() => of(errorAddMessageDefinition())));
        }));
        this.updateMsgDef = this.actions$
            .pipe(ofType(updateMessageDefinition), mergeMap((evt) => {
            return this.vpnService.updateMessageDef(evt.vpn, evt.applicationDomainId, evt.messageDefId, evt.description, evt.jsonSchema)
                .pipe(map(content => completeUpdateMessageDefinition({ vpn: evt.vpn, messageDefId: evt.messageDefId, applicationDomainId: evt.applicationDomainId, description: evt.description, jsonSchema: evt.jsonSchema })), catchError(() => of(errorUpdateMessageDefinition())));
        }));
        this.publishMsgDef = this.actions$
            .pipe(ofType(publishMessageDefinition), mergeMap((evt) => {
            return this.vpnService.publishMessageDef(evt.vpn, evt.applicationDomainId, evt.messageName)
                .pipe(map(content => completePublishMessageDefinition({ vpn: evt.vpn, applicationDomainId: evt.applicationDomainId, messageName: evt.messageName, newMessageDefId: content.id })), catchError(() => of(errorPublishMessageDefinition())));
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
__decorate([
    Effect()
], VpnEffects.prototype, "getVpn", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "getAllClients", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "getClient", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "deleteClient", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "addClient", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "addAppDomain", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "getAppDomains", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "getAppDomain", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "deleteAppDomain", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "getLatestMessages", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "addMsgDef", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "updateMsgDef", void 0);
__decorate([
    Effect()
], VpnEffects.prototype, "publishMsgDef", void 0);
VpnEffects = __decorate([
    Injectable()
], VpnEffects);
export { VpnEffects };
//# sourceMappingURL=vpn.effects.js.map