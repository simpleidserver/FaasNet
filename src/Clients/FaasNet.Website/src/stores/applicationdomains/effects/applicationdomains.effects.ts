import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddAppDomain, completeDeleteAppDomain, completeGetAllAppDomains, completeGetAppDomain, completeUpdateApplicationDomain, errorAddAppDomain, errorDeleteAppDomain, errorGetAllAppDomains, errorGetAppDomain, errorUpdateApplicationDomain, startAddAppDomain, startDeleteAppDomain, startGetAllAppDomains, startGetAppDomain, updateApplicationDomain } from '../actions/applicationdomains.actions';
import { ApplicationResult } from '../models/application.model';
import { ApplicationDomainService } from '../services/applicationdomain.service';

@Injectable()
export class ApplicationEffects {
  constructor(
    private actions$: Actions,
    private applicationService: ApplicationDomainService,
  ) { }

  @Effect()
  getAppDomain = this.actions$
    .pipe(
      ofType(startGetAppDomain),
      mergeMap((evt: { id: string }) => {
        return this.applicationService.getAppDomain(evt.id)
          .pipe(
            map(content => completeGetAppDomain({ content: content })),
            catchError(() => of(errorGetAppDomain()))
          );
      }
      )
  );

  @Effect()
  getAllAppDomains = this.actions$
    .pipe(
      ofType(startGetAllAppDomains),
      mergeMap((evt: { vpn: string }) => {
        return this.applicationService.getAppDomains(evt.vpn)
          .pipe(
            map(content => completeGetAllAppDomains({ content: content })),
            catchError(() => of(errorGetAllAppDomains()))
          );
      }
      )
    );

  @Effect()
  addAppDomain = this.actions$
    .pipe(
      ofType(startAddAppDomain),
      mergeMap((evt: { vpn: string, rootTopic: string, name: string, description: string }) => {
        return this.applicationService.addAppDomain(evt.vpn, evt.rootTopic, evt.name, evt.description)
          .pipe(
            map(content => completeAddAppDomain({ id: content.Id, description: evt.description, name : evt.name, rootTopic: evt.rootTopic })),
            catchError(() => of(errorAddAppDomain()))
            );
      }
      )
  );

  @Effect()
  deleteAppDomain = this.actions$
    .pipe(
      ofType(startDeleteAppDomain),
      mergeMap((evt : { id: string }) => {
        return this.applicationService.deleteApplicationDomain(evt.id)
          .pipe(
            map(content => completeDeleteAppDomain({ id: evt.id })),
            catchError(() => of(errorDeleteAppDomain()))
          );
      }
      )
  );

  @Effect()
  updateAppDomain = this.actions$
    .pipe(
      ofType(updateApplicationDomain),
      mergeMap((evt: { id: string, applications: ApplicationResult[]}) => {
        return this.applicationService.updateApplicationDomain(evt.id, evt.applications)
          .pipe(
            map(content => completeUpdateApplicationDomain({ id: evt.id, applications: evt.applications })),
            catchError(() => of(errorUpdateApplicationDomain()))
          );
      }
      )
    );
}
