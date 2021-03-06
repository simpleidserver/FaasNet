import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { updateApplicationDomain } from '@stores/applicationdomains/actions/applicationdomains.actions';
import { AppDomainResult } from '@stores/applicationdomains/models/appdomain.model';
import { ApplicationResult } from '@stores/applicationdomains/models/application.model';
import * as fromReducers from '@stores/appstate';
import { filter } from 'rxjs/operators';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';
import { getLatestMessages } from '@stores/messagedefinitions/actions/messagedefs.actions';

@Component({
  selector: 'editor-domain',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss']
})
export class EditorDomainComponent implements OnInit {
  vpnName: string = "";
  appDomainId: string = "";
  rootTopic: string = "";
  applications: ApplicationResult[] = [];
  messages: MessageDefinitionResult[] = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) { }

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
    this.vpnName = this.activatedRoute.parent?.snapshot.params['vpnName'];
    this.appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
    const getMessages = getLatestMessages({ appDomainId: this.appDomainId });
    this.store.dispatch(getMessages);
  }

  save() {
    const act = updateApplicationDomain({ id: this.appDomainId, applications: this.applications });
    this.store.dispatch(act);
  }
}
