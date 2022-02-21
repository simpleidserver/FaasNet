import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeGetAll, errorAdd, errorGetAll, startAdd, startGetAll } from '../actions/eventmeshserver.actions';
import { EventMeshServerService } from '../services/eventmeshserver.service';

@Injectable()
export class EventMeshServerEffects {
  constructor(
    private actions$: Actions,
    private eventMeshServerService: EventMeshServerService,
  ) { }

  @Effect()
  addEventMeshServer = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt: { isLocalhost: boolean, urn: string, port: number }) => {
        return this.eventMeshServerService.add(evt.isLocalhost, evt.urn, evt.port)
          .pipe(
            map(content => completeAdd({ content: content })),
            catchError(() => of(errorAdd()))
            );
      }
      )
  );

  @Effect()
  getAllEventMeshServers = this.actions$
    .pipe(
      ofType(startGetAll),
      mergeMap(() => {
        return this.eventMeshServerService.getAll()
          .pipe(
            map(content => completeGetAll({ content: content })),
            catchError(() => of(errorGetAll()))
          );
      }
      )
    );
}
