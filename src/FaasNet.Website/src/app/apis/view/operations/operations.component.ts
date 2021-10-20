import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { startAddOperation, startGet } from '@stores/apis/actions/api.actions';
import { ApiDefinitionOperationResult, ApiDefinitionResult } from '@stores/apis/models/apidef.model';
import * as fromReducers from '@stores/appstate';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddOperationComponent } from './add-operation.component';

@Component({
  selector: 'list-apioperations',
  templateUrl: './operations.component.html'
})
export class OperationsApiComponents implements OnInit {
  displayedColumns: string[] = ['name', 'path', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  operations: ApiDefinitionOperationResult[] = [];
  name: string = "";
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[ApiDefs] COMPLETE_ADD_APIDEF_OPERATION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('apis.messages.apiOperationAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[ApiDefs] ERROR_ADD_APIDEF_OPERATION'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('apis.messages.errorAddApiOperation'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state: ApiDefinitionResult | null) => {
      if (!state) {
        return;
      }

      this.operations = state.operations;
    });
  }

  ngAfterViewInit() {
    if (!this.sort || !this.paginator) {
      return;
    }

    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  addOperation() {
    const dialogRef = this.dialog.open(AddOperationComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt) {
        return;
      }

      const addOperation = startAddOperation({ funcName: this.name, opName: opt.name, opPath: opt.path });
      this.store.dispatch(addOperation);
    });
  }

  private refresh() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    this.name = name;
    const action = startGet({ funcName: name });
    this.store.dispatch(action);
  }
}
