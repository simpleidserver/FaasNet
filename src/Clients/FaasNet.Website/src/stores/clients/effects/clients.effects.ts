import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddClient, completeDeleteClient, completeGetAllClients, completeGetClient, errorAddClient, errorDeleteClient, errorGetAllClients, errorGetClient, startAddClient, startDeleteClient, startGetAllClients, startGetClient } from '../actions/clients.actions';
import { ClientService } from '../services/clients.service';

@Injectable()
export class ClientEffects {
  constructor(
    private actions$: Actions,
    private clientService: ClientService,
  ) { }

  @Effect()
  getAllClients = this.actions$
    .pipe(
      ofType(startGetAllClients),
      mergeMap((evt: { vpn: string }) => {
        return this.clientService.getAllClients(evt.vpn)
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
      mergeMap((evt: { id: string }) => {
        return this.clientService.getClient(evt.id)
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
      mergeMap((evt: { id: string }) => {
        return this.clientService.deleteClient(evt.id)
          .pipe(
            map(content => completeDeleteClient({ id: evt.id })),
            catchError(() => of(errorDeleteClient()))
            );
      }
      )
  );

  @Effect()
  addClient = this.actions$
    .pipe(
      ofType(startAddClient),
      mergeMap((evt: { vpn: string, clientId: string, purposes: number[] }) => {
        return this.clientService.addClient(evt.vpn, evt.clientId, evt.purposes)
          .pipe(
            map(content => completeAddClient({ id: content.Id, clientId : evt.clientId, purposes : evt.purposes, vpn: evt.vpn })),
            catchError(() => of(errorAddClient()))
          );
      }
      )
  );
}
