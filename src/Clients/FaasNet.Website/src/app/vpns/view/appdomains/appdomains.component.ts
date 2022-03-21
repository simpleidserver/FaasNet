import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startAddAppDomain, startDeleteAppDomain, startGetAppDomains } from '@stores/vpn/actions/vpn.actions';
import { AppDomainResult } from '@stores/vpn/models/appdomain.model';
import { filter } from 'rxjs/operators';
import { AddAppDomainComponent } from './add-appdomain.component';

@Component({
  selector: 'appdomains-vpn',
  templateUrl: './appdomains.component.html',
  styleUrls: ['./appdomains.component.scss']
})
export class AppDomainsVpnComponent implements OnInit {
  appDomains: AppDomainResult[] = [];
  displayedColumns: string[] = ['actions', 'name', 'description', 'rootTopic', 'createDateTime', 'updateDateTime'];
  name: string = "";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService,
    private dialog : MatDialog  ) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_ADD_APPDOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.appDomainAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_ADD_APPDOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorAddAppDomain'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_DELETE_APPDOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.appDomainRemoved'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_DELETE_APPDOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveAppDomain'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectAppDomainsResult)).subscribe((state: AppDomainResult[] | null) => {
      if (!state) {
        return;
      }

      this.appDomains = state;
    });
    this.refresh();
  }

  addAppDomain() {
    const dialogRef = this.dialog.open(AddAppDomainComponent, {
      width : '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const act = startAddAppDomain({ vpn: this.name, name: e.name, description: e.description, rootTopic: e.rootTopic });
      this.store.dispatch(act);
    });
  }

  removeAppDomain(appDomain: AppDomainResult) {
    const act = startDeleteAppDomain({ name: this.name, appDomainId: appDomain.id });
    this.store.dispatch(act);
  }

  private refresh() {
    this.name = this.activatedRoute.parent?.snapshot.params['name'];
    const act = startGetAppDomains({ name: this.name });
    this.store.dispatch(act);
  }
}
