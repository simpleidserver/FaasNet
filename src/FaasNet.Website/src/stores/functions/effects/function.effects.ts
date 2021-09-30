import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeGetConfiguration,
    completeSearch, errorGetConfiguration, errorSearch, startGetConfiguration, startSearch
} from '../actions/function.actions';
import { FunctionService } from '../services/function.service';

@Injectable()
export class FunctionEffects {
  constructor(
    private actions$: Actions,
    private applicationService: FunctionService,
  ) { }

  @Effect()
  searchFunctions$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt: { order: string, direction: string, count: number, startIndex: number }) => {
        return this.applicationService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
  );

  @Effect()
  getFunctionConfiguration$ = this.actions$
    .pipe(
      ofType(startGetConfiguration),
      mergeMap((evt: { name: string }) => {
        return this.applicationService.getConfiguration(evt.name)
          .pipe(
            map(content => completeGetConfiguration({ content: content })),
            catchError(() => of(errorGetConfiguration()))
          );
      }
      )
    );
}
