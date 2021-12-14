import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddEmpty, completeGetJson, completeLaunch, completeSearch, completeUpdate, errorAddEmpty, errorGetJson, errorLaunch, errorSearch, errorUpdate, startAddEmpty, startGetJson, startLaunch, startSearch, startUpdate } from '../actions/statemachines.actions';
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
      mergeMap((evt: { id: string, stateMachine: StateMachineModel }) => {
        return this.stateMachinesService.update(evt.id, evt.stateMachine)
          .pipe(
            map(content => completeUpdate({ id: content.id })),
            catchError(() => of(errorUpdate()))
          );
      }
      )
  );

  @Effect()
  launchStateMachine = this.actions$
    .pipe(
      ofType(startLaunch),
      mergeMap((evt: { id: string, input : any }) => {
        return this.stateMachinesService.launch(evt.id, evt.input)
          .pipe(
            map(content => completeLaunch({ id: content.id, launchDateTime: content.launchDateTime })),
            catchError(() => of(errorLaunch()))
          );
      }
      )
    );
}
