import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startAdd, startSearch } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddFunctionComponent } from './add-function.component';

@Component({
  selector: 'list-functions',
  templateUrl: './list.component.html'
})
export class ListFunctionsComponent implements OnInit {
  displayedColumns: string[] = ['name', 'description', 'image', 'version', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  functions: FunctionResult[] = [];
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] COMPLETE_ADD_FUNCTION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('functions.messages.functionAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] ERROR_ADD_FUNCTION'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('functions.messages.errorAddFunction'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectFunctionsResult)).subscribe((state: SearchResult<FunctionResult> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.functions = state.content;
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

  addFunction() {
    const dialogRef = this.dialog.open(AddFunctionComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt) {
        return;
      }

      const addFunction = startAdd({ name: opt.name, description: opt.description, image: opt.image, version: opt.version });
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
