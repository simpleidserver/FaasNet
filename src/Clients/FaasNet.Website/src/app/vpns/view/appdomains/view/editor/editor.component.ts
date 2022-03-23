import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { updateApplicationDomain } from '@stores/vpn/actions/vpn.actions';
import { AppDomainResult } from '@stores/vpn/models/appdomain.model';
import { ApplicationResult } from '@stores/vpn/models/application.model';
import { filter } from 'rxjs/operators';

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

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_UPDATE_APPLICATION_DOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.applicationDomainUpdated'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_UPDATE_APPLICATION_DOMAIN'))
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
    this.vpnName = this.activatedRoute.parent?.snapshot.params['vpnName'];
    this.appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
  }

  save() {
    const act = updateApplicationDomain({ vpn: this.vpnName, applicationDomainId: this.appDomainId, applications: this.applications });
    this.store.dispatch(act);
  }
}
