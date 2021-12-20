import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeAdd,
    completeDelete,
    completeGet,
    completeGetConfiguration,
    completeGetCpuUsage,
    completeGetDetails,
    completeGetRequestDuration,
    completeGetThreads,
    completeGetTotalRequests,
    completeGetVirtualMemoryBytes,
    completeInvoke,
    completeSearch, errorAdd, errorDelete, errorGet, errorGetConfiguration, errorGetCpuUsage, errorGetDetails, errorGetRequestDuration, errorGetThreads, errorGetTotalRequests, errorGetVirtualMemoryBytes, errorInvoke, errorSearch, startAdd, startDelete, startGet, startGetConfiguration, startGetCpuUsage, startGetDetails, startGetRequestDuration, startGetThreads, startGetTotalRequests, startGetVirtualMemoryBytes, startInvoke, startSearch
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

  @Effect()
  invokeFunction$ = this.actions$
    .pipe(
      ofType(startInvoke),
      mergeMap((evt: { name: string, request: any }) => {
        return this.applicationService.invoke(evt.name, evt.request)
          .pipe(
            map(content => completeInvoke({ content: content })),
            catchError(() => of(errorInvoke()))
          );
      }
      )
  );

  @Effect()
  getFunction$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt: { name: string }) => {
        return this.applicationService.get(evt.name)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  deleteFunction$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt: { name: string}) => {
        return this.applicationService.delete(evt.name)
          .pipe(
            map(content => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
  );

  @Effect()
  addFunction$ = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt: { name: string, description: string, image: string, version: string }) => {
        return this.applicationService.add(evt.name, evt.description, evt.image, evt.version)
          .pipe(
            map(content => completeAdd({ name: evt.name, image: evt.image, description: evt.description, version: evt.version })),
            catchError(() => of(errorAdd()))
          );
      }
      )
  );

  @Effect()
  getThreads$ = this.actions$
    .pipe(
      ofType(startGetThreads),
      mergeMap((evt: { name: string, startDate: number, endDate: number }) => {
        return this.applicationService.getThreads(evt.name, evt.startDate, evt.endDate)
          .pipe(
            map(content => completeGetThreads({ content: content })),
            catchError(() => of(errorGetThreads()))
          );
      }
      )
  );

  @Effect()
  getVirtualMemoryBytes$ = this.actions$
    .pipe(
      ofType(startGetVirtualMemoryBytes),
      mergeMap((evt: { name: string, startDate: number, endDate: number }) => {
        return this.applicationService.getVirtualMemoryBytes(evt.name, evt.startDate, evt.endDate)
          .pipe(
            map(content => completeGetVirtualMemoryBytes({ content: content })),
            catchError(() => of(errorGetVirtualMemoryBytes()))
          );
      }
      )
  );

  @Effect()
  getCpuUsage$ = this.actions$
    .pipe(
      ofType(startGetCpuUsage),
      mergeMap((evt: { name: string, startDate: number, endDate: number, duration: number}) => {
        return this.applicationService.getCpuUsage(evt.name, evt.startDate, evt.endDate, evt.duration)
          .pipe(
            map(content => completeGetCpuUsage({ content: content })),
            catchError(() => of(errorGetCpuUsage()))
          );
      }
      )
  );

  @Effect()
  getRequestDuration$ = this.actions$
    .pipe(
      ofType(startGetRequestDuration),
      mergeMap((evt: { name: string, startDate: number, endDate: number, duration: number }) => {
        return this.applicationService.getRequestDuration(evt.name, evt.startDate, evt.endDate, evt.duration)
          .pipe(
            map(content => completeGetRequestDuration({ content: content })),
            catchError(() => of(errorGetRequestDuration()))
          );
      }
      )
  );

  @Effect()
  getDetails$ = this.actions$
    .pipe(
      ofType(startGetDetails),
      mergeMap((evt: { name: string }) => {
        return this.applicationService.getDetails(evt.name)
          .pipe(
            map(content => completeGetDetails({ content: content })),
            catchError(() => of(errorGetDetails()))
          );
      }
      )
  );

  @Effect()
  getTotalRequests$ = this.actions$
    .pipe(
      ofType(startGetTotalRequests),
      mergeMap((evt: { name: string, time: number }) => {
        return this.applicationService.getTotalRequests(evt.name, evt.time)
          .pipe(
            map(content => completeGetTotalRequests({ content: content })),
            catchError(() => of(errorGetTotalRequests()))
          );
      }
      )
    );
}
