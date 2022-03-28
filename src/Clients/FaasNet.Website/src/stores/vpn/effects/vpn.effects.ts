import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddVpn, completeDeleteVpn, completeGetAllVpn, completeGetVpn, deleteVpn, errorAddVpn, errorGetAllVpn, errorGetVpn, startAddVpn, startGetAllVpn, startGetVpn } from '../actions/vpn.actions';
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
}
