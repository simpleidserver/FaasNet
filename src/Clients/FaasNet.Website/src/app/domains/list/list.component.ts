import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { startAddApplicationDomain, startGetAllApplicationDomains } from '@stores/application/actions/application.actions';
import * as fromReducers from '@stores/appstate';
import { filter } from 'rxjs/operators';
import { ApplicationDomainResult } from '../../../stores/application/models/applicationdomain.model';
import { AddApplicationDomainComponent } from './add-applicationdomain.component';
declare var ol: any;
declare var $: any;

@Component({
  selector: 'list-applicationdomains',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListApplicationDomainComponent implements OnInit {
  displayedColumns: string[] = ['name', 'description', 'version', 'createDateTime', 'updateDateTime'];
  isLoading: boolean = false;
  applicationDomains: ApplicationDomainResult[] = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_ADD_APPLICATION_DOMAIN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('applicationDomains.messages.applicationDomainAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_ADD_APPLICATION_DOMAIN'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('applicationDomains.messages.errorAddApplicationDomain'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.store.pipe(select(fromReducers.selectApplicationDomainsResult)).subscribe((state: ApplicationDomainResult[] | null) => {
      if (!state) {
        return;
      }

      this.applicationDomains = state;
      this.isLoading = false;
    });
    this.refresh();
  }

  addApplicationDomain() {
    const dialogRef = this.dialog.open(AddApplicationDomainComponent);
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      this.isLoading = true;
      const addApplicationDomain = startAddApplicationDomain({ rootTopic: e.rootTopic, name: e.name, description: e.description });
      this.store.dispatch(addApplicationDomain);
    });
  }

  private refresh() {
    this.isLoading = true;
    const getAllApplicationDomain = startGetAllApplicationDomains();
    this.store.dispatch(getAllApplicationDomain);
  }
}
