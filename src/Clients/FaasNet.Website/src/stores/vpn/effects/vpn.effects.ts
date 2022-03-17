import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddClient, completeAddVpn, completeDeleteClient, completeDeleteVpn, completeGetAllClients, completeGetAllVpn, completeGetClient, completeGetVpn, deleteVpn, errorAddClient, errorAddVpn, errorDeleteClient, errorGetAllClients, errorGetAllVpn, errorGetClient, errorGetVpn, startAddClient, startAddVpn, startGetAllClients, startGetAllVpn, startGetClient, startGetVpn } from '../actions/vpn.actions';
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
      ofType(startGetClient),
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
}
