import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { addMessageDefinition, completeAddAppDomain, completeAddClient, completeAddMessageDefinition, completeAddVpn, completeDeleteAppDomain, completeDeleteClient, completeDeleteVpn, completeGetAllClients, completeGetAllVpn, completeGetAppDomain, completeGetAppDomains, completeGetClient, completeGetLatestMessages, completeGetVpn, completePublishMessageDefinition, completeUpdateMessageDefinition, deleteVpn, errorAddAppDomain, errorAddClient, errorAddMessageDefinition, errorAddVpn, errorDeleteAppDomain, errorDeleteClient, errorGetAllClients, errorGetAllVpn, errorGetAppDomain, errorGetAppDomains, errorGetClient, errorGetLatestMessages, errorGetVpn, errorPublishMessageDefinition, errorUpdateMessageDefinition, getLatestMessages, publishMessageDefinition, startAddAppDomain, startAddClient, startAddVpn, startDeleteAppDomain, startDeleteClient, startGetAllClients, startGetAllVpn, startGetAppDomain, startGetAppDomains, startGetClient, startGetVpn, updateMessageDefinition } from '../actions/vpn.actions';
import { VpnService } from '../services/vpn.service';

@Injectable()
export class VpnEffects {
  constructor(
    private actions$: Actions,
    private vpnService: VpnService,
  ) { }

  @Effect()
  getAllVpn = this.actions$
    .pipe(
      ofType(startGetAllVpn),
      mergeMap(() => {
        return this.vpnService.getAllVpn()
          .pipe(
            map(content => completeGetAllVpn({ content: content })),
            catchError(() => of(errorGetAllVpn()))
          );
      }
      )
  );

  @Effect()
  addVpn = this.actions$
    .pipe(
      ofType(startAddVpn),
      mergeMap((evt: { name: string, description: string }) => {
        return this.vpnService.addVpn(evt.name, evt.description)
          .pipe(
            map(content => completeAddVpn(evt)),
            catchError(() => of(errorAddVpn()))
          );
      }
      )
  );

  @Effect()
  deleteVpn = this.actions$
    .pipe(
      ofType(deleteVpn),
      mergeMap((evt: { name: string }) => {
        return this.vpnService.deleteVpn(evt.name)
          .pipe(
            map(content => completeDeleteVpn(evt)),
            catchError(() => of(errorAddVpn()))
          );
      }
      )
  );

  @Effect()
  getVpn = this.actions$
    .pipe(
      ofType(startGetVpn),
      mergeMap((evt: { name: string }) => {
        return this.vpnService.getVpn(evt.name)
          .pipe(
            map(content => completeGetVpn({ content: content })),
            catchError(() => of(errorGetVpn()))
          );
      }
      )
  );

  @Effect()
  getAllClients = this.actions$
    .pipe(
      ofType(startGetAllClients),
      mergeMap((evt: { name: string }) => {
        return this.vpnService.getAllClients(evt.name)
          .pipe(
            map(content => completeGetAllClients({ content: content })),
            catchError(() => of(errorGetAllClients()))
          );
      }
      )
  );

  @Effect()
  getClient = this.actions$
    .pipe(
      ofType(startGetClient),
      mergeMap((evt: { name: string, clientId: string }) => {
        return this.vpnService.getClient(evt.name, evt.clientId)
          .pipe(
            map(content => completeGetClient({ content: content })),
            catchError(() => of(errorGetClient()))
          );
      }
      )
  );

  @Effect()
  deleteClient = this.actions$
    .pipe(
      ofType(startDeleteClient),
      mergeMap((evt: { name: string, clientId: string }) => {
        return this.vpnService.deleteClient(evt.name, evt.clientId)
          .pipe(
            map(content => completeDeleteClient(evt)),
            catchError(() => of(errorDeleteClient()))
          );
      }
      )
  );

  @Effect()
  addClient = this.actions$
    .pipe(
      ofType(startAddClient),
      mergeMap((evt: { name: string, clientId: string, purposes: number[] }) => {
        return this.vpnService.addClient(evt.name, evt.clientId, evt.purposes)
          .pipe(
            map(content => completeAddClient(evt)),
            catchError(() => of(errorAddClient()))
          );
      }
      )
  );

  @Effect()
  addAppDomain = this.actions$
    .pipe(
      ofType(startAddAppDomain),
      mergeMap((evt: { vpn: string, name: string, description: string, rootTopic: string }) => {
        return this.vpnService.addAppDomain(evt.vpn, evt.name, evt.description, evt.rootTopic)
          .pipe(
            map(content => completeAddAppDomain({ id: content.id, name: evt.name, description: evt.description, rootTopic: evt.rootTopic })),
            catchError(() => of(errorAddAppDomain()))
          );
      }
      )
  );

  @Effect()
  getAppDomains = this.actions$
    .pipe(
      ofType(startGetAppDomains),
      mergeMap((evt: { name: string }) => {
        return this.vpnService.getAppDomains(evt.name)
          .pipe(
            map(content => completeGetAppDomains({ content: content })),
            catchError(() => of(errorGetAppDomains()))
          );
      }
      )
  );

  @Effect()
  getAppDomain = this.actions$
    .pipe(
      ofType(startGetAppDomain),
      mergeMap((evt: { name: string, appDomainId: string }) => {
        return this.vpnService.getAppDomain(evt.name, evt.appDomainId)
          .pipe(
            map(content => completeGetAppDomain({ content: content })),
            catchError(() => of(errorGetAppDomain()))
          );
      }
      )
    );

  @Effect()
  deleteAppDomain = this.actions$
    .pipe(
      ofType(startDeleteAppDomain),
      mergeMap((evt: { name: string, appDomainId: string }) => {
        return this.vpnService.deleteAppDomain(evt.name, evt.appDomainId)
          .pipe(
            map(content => completeDeleteAppDomain({ name: evt.name, appDomainId: evt.appDomainId})),
            catchError(() => of(errorDeleteAppDomain()))
          );
      }
      )
  );

  @Effect()
  getLatestMessages = this.actions$
    .pipe(
      ofType(getLatestMessages),
      mergeMap((evt: { name: string, appDomainId: string }) => {
        return this.vpnService.getLatestMessagesDef(evt.name, evt.appDomainId)
          .pipe(
            map(content => completeGetLatestMessages({ content: content })),
            catchError(() => of(errorGetLatestMessages()))
          );
      }
      )
  );

  @Effect()
  addMsgDef = this.actions$
    .pipe(
      ofType(addMessageDefinition),
      mergeMap((evt: { vpn: string, applicationDomainId: string, name: string, description: string, jsonSchema: string }) => {
        return this.vpnService.addMessageDef(evt.vpn, evt.applicationDomainId, evt.name, evt.description, evt.jsonSchema)
          .pipe(
            map(content => completeAddMessageDefinition({ vpn: evt.vpn, messageDefId: content.id, applicationDomainId: evt.applicationDomainId, description: evt.description, jsonSchema: evt.jsonSchema, name: evt.name })),
            catchError(() => of(errorAddMessageDefinition()))
          );
      }
      )
  );

  @Effect()
  updateMsgDef = this.actions$
    .pipe(
      ofType(updateMessageDefinition),
      mergeMap((evt: { vpn: string, applicationDomainId: string, messageDefId: string, description: string, jsonSchema: string }) => {
        return this.vpnService.updateMessageDef(evt.vpn, evt.applicationDomainId, evt.messageDefId, evt.description, evt.jsonSchema)
          .pipe(
            map(content => completeUpdateMessageDefinition({ vpn: evt.vpn, messageDefId: evt.messageDefId, applicationDomainId: evt.applicationDomainId, description: evt.description, jsonSchema: evt.jsonSchema })),
            catchError(() => of(errorUpdateMessageDefinition()))
          );
      }
      )
  );

  @Effect()
  publishMsgDef = this.actions$
    .pipe(
      ofType(publishMessageDefinition),
      mergeMap((evt: { vpn: string, applicationDomainId: string, messageName: string }) => {
        return this.vpnService.publishMessageDef(evt.vpn, evt.applicationDomainId, evt.messageName)
          .pipe(
            map(content => completePublishMessageDefinition({ vpn: evt.vpn, applicationDomainId: evt.applicationDomainId, messageName: evt.messageName, newMessageDefId: content.id })),
            catchError(() => of(errorPublishMessageDefinition()))
          );
      }
      )
    );
}
