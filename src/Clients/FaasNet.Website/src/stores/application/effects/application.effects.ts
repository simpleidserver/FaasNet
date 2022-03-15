import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAddApplicationDomain, completeGetAllApplicationDomains, errorAddApplicationDomain, errorGetAllApplicationDomains, startAddApplicationDomain, startGetAllApplicationDomains } from '../actions/application.actions';
import { ApplicationService } from '../services/eventmeshserver.service';

@Injectable()
export class ApplicationEffects {
  constructor(
    private actions$: Actions,
    private applicationService: ApplicationService,
  ) { }

  @Effect()
  addEventMeshServer = this.actions$
    .pipe(
      ofType(startAddApplicationDomain),
      mergeMap((evt: { rootTopic: string, name: string, description: string }) => {
        return this.applicationService.addApplicationDomain(evt.rootTopic, evt.name, evt.description)
          .pipe(
            map(content => completeAddApplicationDomain({ content: content })),
            catchError(() => of(errorAddApplicationDomain()))
            );
      }
      )
  );

  @Effect()
  getAllApplicationDomains = this.actions$
    .pipe(
      ofType(startGetAllApplicationDomains),
      mergeMap(() => {
        return this.applicationService.getAllApplicationDomains()
          .pipe(
            map(content => completeGetAllApplicationDomains({ content: content })),
            catchError(() => of(errorGetAllApplicationDomains()))
          );
      }
      )
  );
}
