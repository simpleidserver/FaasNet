import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGet, completeReactivate, completeSearch, errorGet, errorReactivate, errorSearch, startGet, startReactivate, startSearch } from '../actions/statemachineinstances.actions';
import { StateMachineInstancesService } from '../services/statemachineinstances.service';

@Injectable()
export class StateMachineInstancesEffects {
  constructor(
    private actions$: Actions,
    private stateMachineInstancesService: StateMachineInstancesService,
  ) { }

  @Effect()
  searchStateMachineInstances$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt: { order: string, direction: string, count: number, startIndex: number, vpn: string }) => {
        return this.stateMachineInstancesService.search(evt.startIndex, evt.count, evt.order, evt.direction, evt.vpn)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
  );

  @Effect()
  getStateMachineInstance$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt: { id: string }) => {
        return this.stateMachineInstancesService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  reactivateMachineInstance = this.actions$
    .pipe(
      ofType(startReactivate),
      mergeMap((evt: { id: string }) => {
        return this.stateMachineInstancesService.reactivate(evt.id)
          .pipe(
            map(content => completeReactivate()),
            catchError(() => of(errorReactivate()))
          );
      }
      )
    );
}
