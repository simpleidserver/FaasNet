import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { addMessageDefinition, completeAddMessageDefinition, completeGetLatestMessages, completePublishMessageDefinition, completeUpdateMessageDefinition, errorAddMessageDefinition, errorGetLatestMessages, errorPublishMessageDefinition, errorUpdateMessageDefinition, getLatestMessages, publishMessageDefinition, updateMessageDefinition } from '../actions/messagedefs.actions';
import { MessageDefsService } from '../services/messagedefs.service';

@Injectable()
export class MessageDefEffects {
  constructor(
    private actions$: Actions,
    private messageDefsService: MessageDefsService,
  ) { }

  @Effect()
  getLatestMessages = this.actions$
    .pipe(
      ofType(getLatestMessages),
      mergeMap((evt: { appDomainId: string }) => {
        return this.messageDefsService.getLatestMessagesDef(evt.appDomainId)
          .pipe(
            map(content => completeGetLatestMessages({ content: content })),
            catchError(() => of(errorGetLatestMessages()))
          );
      }
      )
  );

  @Effect()
  addMessageDefinition = this.actions$
    .pipe(
      ofType(addMessageDefinition),
      mergeMap((evt: { appDomainId: string, name: string, description: string, jsonSchema: string }) => {
        return this.messageDefsService.addMessageDef(evt.appDomainId, evt.name, evt.description, evt.jsonSchema)
          .pipe(
            map(content => completeAddMessageDefinition({ id: content.Id, appDomainId: evt.appDomainId, description : evt.description, jsonSchema : evt.jsonSchema, name: evt.name })),
            catchError(() => of(errorAddMessageDefinition()))
          );
      }
      )
    );

  @Effect()
  updateMessageDefinition = this.actions$
    .pipe(
      ofType(updateMessageDefinition),
      mergeMap((evt: { id: string, description: string, jsonSchema: string }) => {
        return this.messageDefsService.updateMessageDef(evt.id, evt.description, evt.jsonSchema)
          .pipe(
            map(content => completeUpdateMessageDefinition({ id : evt.id, description : evt.description, jsonSchema: evt.jsonSchema })),
            catchError(() => of(errorUpdateMessageDefinition()))
            );
      }
      )
  );

  @Effect()
  publishMessageDefinition = this.actions$
    .pipe(
      ofType(publishMessageDefinition),
      mergeMap((evt : { id: string }) => {
        return this.messageDefsService.publishMessageDef(evt.id)
          .pipe(
            map(content => completePublishMessageDefinition({ id: evt.id, newId: content.Id })),
            catchError(() => of(errorPublishMessageDefinition()))
          );
      }
      )
  );
}
