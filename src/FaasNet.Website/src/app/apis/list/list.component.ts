import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { startAdd, startSearch } from '@stores/apis/actions/api.actions';
import { ApiDefinitionResult } from '@stores/apis/models/apidef.model';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddApiDefComponent } from './add-api.component';

@Component({
  selector: 'list-apidefs',
  templateUrl: './list.component.html'
})
export class ListApiDefComponent implements OnInit {
  displayedColumns: string[] = ['name', 'path', 'nbOperations', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  apiDefs: ApiDefinitionResult[] = [];
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[ApiDefs] COMPLETE_ADD_APIDEF'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('apis.messages.apiAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] ERROR_ADD_APIDEF'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('apis.messages.errorAddApi'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectApiDefsResult)).subscribe((state: SearchResult<ApiDefinitionResult> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.apiDefs = state.content;
      this.length = state.totalLength;
    });
  }

  ngAfterViewInit() {
    if (!this.sort || !this.paginator) {
      return;
    }

    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  addApi() {
    const dialogRef = this.dialog.open(AddApiDefComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt) {
        return;
      }

      const addFunction = startAdd({ name: opt.name, path: opt.path });
      this.store.dispatch(addFunction);
    });
  }

  private refresh() {
    if (!this.paginator || !this.sort) {
      return;
    }

    let startIndex: number = 0;
    let count: number = 5;
    if (this.paginator.pageIndex && this.paginator.pageSize) {
      startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    }

    if (this.paginator.pageSize) {
      count = this.paginator.pageSize;
    }

    let active = "createDateTime";
    let direction = "desc";
    if (this.sort.active) {
      active = this.sort.active;
    }

    if (this.sort.direction) {
      direction = this.sort.direction;
    }

    let request = startSearch({ order: active, direction, count, startIndex });
    this.store.dispatch(request);
  }
}
