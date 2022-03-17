import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGetServerStatus, errorGetServerStatus, startGetServerStatus } from '../actions/server.actions';
import { ServerService } from '../services/server.service';

@Injectable()
export class ServerEffects {
  constructor(
    private actions$: Actions,
    private serverService: ServerService,
  ) { }

  @Effect()
  getStatus = this.actions$
    .pipe(
      ofType(startGetServerStatus),
      mergeMap(() => {
        return this.serverService.getStatus()
          .pipe(
            map(content => completeGetServerStatus({ content: content })),
            catchError(() => of(errorGetServerStatus()))
            );
      }
      )
  );
}
