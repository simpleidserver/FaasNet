import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddEmpty, completeGetJson, completeSearch, completeUpdate, errorAddEmpty, errorGetJson, errorSearch, errorUpdate, startAddEmpty, startGetJson, startSearch, startUpdate } from '../actions/statemachines.actions';
import { StateMachineModel } from '../models/statemachinemodel.model';
import { StateMachinesService } from '../services/statemachines.service';

@Injectable()
export class StateMachineEffects {
  constructor(
    private actions$: Actions,
    private stateMachinesService: StateMachinesService,
  ) { }

  @Effect()
  searchStateMachines = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt: { order: string, direction: string, count: number, startIndex: number }) => {
        return this.stateMachinesService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
    );

  @Effect()
  getStateMachineJson = this.actions$
    .pipe(
      ofType(startGetJson),
      mergeMap((evt: { id: string }) => {
        return this.stateMachinesService.getJson(evt.id)
          .pipe(
            map(content => completeGetJson({ content: content })),
            catchError(() => of(errorGetJson()))
          );
      }
      )
  );

  @Effect()
  addEmptyStateMachine = this.actions$
    .pipe(
      ofType(startAddEmpty),
      mergeMap((evt: { name: string, description: string }) => {
        return this.stateMachinesService.addEmpty(evt.name, evt.description)
          .pipe(
            map(content => completeAddEmpty({ id: content.id })),
            catchError(() => of(errorAddEmpty()))
          );
      }
      )
  );

  @Effect()
  updateStateMachine = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt: { stateMachine: StateMachineModel }) => {
        return this.stateMachinesService.update(evt.stateMachine)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
    );
}
