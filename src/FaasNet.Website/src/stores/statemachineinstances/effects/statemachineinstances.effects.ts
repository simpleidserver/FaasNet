import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeSearch, errorSearch, startSearch } from '../actions/statemachineinstances.actions';
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
      mergeMap((evt: { order: string, direction: string, count: number, startIndex: number }) => {
        return this.stateMachineInstancesService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
    );
}
