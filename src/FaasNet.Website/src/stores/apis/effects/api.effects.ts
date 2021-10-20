import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeGet, completeSearch, completeUpdateUIOperation, errorAdd, errorGet, errorAddOperation, errorSearch, errorUpdateUIOperation, startAdd, startAddOperation, startGet, startSearch, startUpdateUIOperation, completeAddOperation } from '../actions/api.actions';
import { ApiDefinitionOperationUIResult } from '../models/apidef.model';
import { ApiDefService } from '../services/api.service';

@Injectable()
export class ApiDefEffects {
  constructor(
    private actions$: Actions,
    private apiDefService: ApiDefService,
  ) { }

  @Effect()
  addApiDef$ = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt: { name: string, path: string }) => {
        return this.apiDefService.add(evt.name, evt.path)
          .pipe(
            map(content => completeAdd()),
            catchError(() => of(errorAdd()))
          );
      }
      )
  );

  @Effect()
  addApiDefOperation$ = this.actions$
    .pipe(
      ofType(startAddOperation),
      mergeMap((evt: { funcName: string, opName: string, opPath: string }) => {
        return this.apiDefService.addOperation(evt.funcName, evt.opName, evt.opPath)
          .pipe(
            map(content => completeAddOperation()),
            catchError(() => of(errorAddOperation()))
          );
      }
      )
  );

  @Effect()
  updateApiDefUI$ = this.actions$
    .pipe(
      ofType(startUpdateUIOperation),
      mergeMap((evt: { funcName: string, operationName: string, ui: ApiDefinitionOperationUIResult }) => {
        return this.apiDefService.updateUIOperation(evt.funcName, evt.operationName, evt.ui)
          .pipe(
            map(content => completeUpdateUIOperation()),
            catchError(() => of(errorUpdateUIOperation()))
          );
      }
      )
    );

  @Effect()
  searchApiDefs$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt: { order: string, direction: string, count: number, startIndex: number }) => {
        return this.apiDefService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
  );

  @Effect()
  getApiDef = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt: { funcName: string }) => {
        return this.apiDefService.get(evt.funcName)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
    );
}
