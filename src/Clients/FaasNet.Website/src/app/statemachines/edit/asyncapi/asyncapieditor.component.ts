import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { startGetAppDomain } from '@stores/applicationdomains/actions/applicationdomains.actions';
import * as fromReducers from '@stores/appstate';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';
import { AppDomainResult } from '@stores/applicationdomains/models/appdomain.model';
import { ApplicationResult } from '@stores/applicationdomains/models/application.model';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';
import { getLatestMessages } from '@stores/messagedefinitions/actions/messagedefs.actions';

@Component({
  selector: 'asyncapieditor-statemachine',
  templateUrl: './asyncapieditor.component.html',
  styleUrls: [ './asyncapieditor.component.scss' ]
})
export class AsyncApiEditorComponent implements OnInit {
  appDomainId: string = "";
  rootTopic: string = "";
  vpnName: string = "";
  applications: ApplicationResult[] = [];
  messages: MessageDefinitionResult[] = [];
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    if (value) {
      this.appDomainId = value.applicationDomainId;
      this.vpnName = value.vpn;
      this.refresh();
    }
  }

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) {
  }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[APPLICATIONDOMAINS] COMPLETE_UPDATE_APPLICATION_DOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.applicationDomainUpdated'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[APPLICATIONDOMAINS] ERROR_UPDATE_APPLICATION_DOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorUpdateApplicationDomain'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectAppDomainResult)).subscribe((state: AppDomainResult | null) => {
      if (!state) {
        return;
      }

      this.rootTopic = state.rootTopic;
      this.applications = JSON.parse(JSON.stringify(state.applications)) as ApplicationResult[];
    });
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this.messages = state;
    });
  }

  private refresh() {
    const getMessages = getLatestMessages({ appDomainId: this._stateMachineModel.applicationDomainId });
    const act = startGetAppDomain({ id: this._stateMachineModel.applicationDomainId });
    this.store.dispatch(act);
    this.store.dispatch(getMessages);
  }
}
