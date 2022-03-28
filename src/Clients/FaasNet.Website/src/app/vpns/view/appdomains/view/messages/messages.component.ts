import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { addMessageDefinition, getLatestMessages, publishMessageDefinition, updateMessageDefinition } from '@stores/messagedefinitions/actions/messagedefs.actions';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';
import { filter } from 'rxjs/operators';
import { AddMessageDefComponent } from './add-message.component';

@Component({
  selector: 'messages-vpn',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesVpnComponent implements OnInit {
  messages: MessageDefinitionResult[] = [];
  vpnName: string = "";
  appDomainId: string = "";
  displayedColumns: string[] = ['actions', 'name', 'description', 'version', 'createDateTime', 'updateDateTime'];

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] COMPLETE_ADD_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.messageDefAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] ERROR_ADD_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorAddMessageDef'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] COMPLETE_UPDATE_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.messagDefUpdated'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] ERROR_UPDATE_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorUpdateMessageDef'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] COMPLETE_PUBLISH_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.messageDefPublished'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[MESSAGEDEFS] ERROR_PUBLISH_MESSAGE_DEFINITION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorPublishMessage'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this.messages = state;
    });
    this.refresh();
  }

  publishMessage(message: MessageDefinitionResult) {
    const act = publishMessageDefinition({ id: message.id, messageName: message.name });
    this.store.dispatch(act);
  }

  editMessage(message: MessageDefinitionResult) {
    const dialogRef = this.dialog.open(AddMessageDefComponent, {
      width: '800px',
      data: message
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const act = updateMessageDefinition({ id: message.id, description: e.description, jsonSchema: e.jsonSchema });
      this.store.dispatch(act);
    });
  }

  addMessage() {
    const dialogRef = this.dialog.open(AddMessageDefComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const act = addMessageDefinition({ appDomainId: this.appDomainId, description: e.description, name: e.name, jsonSchema: e.jsonSchema });
      this.store.dispatch(act);
    });
  }

  private refresh() {
    this.vpnName = this.activatedRoute.parent?.snapshot.params['vpnName'];
    this.appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
    const act = getLatestMessages({ appDomainId: this.appDomainId});
    this.store.dispatch(act);
  }
}
