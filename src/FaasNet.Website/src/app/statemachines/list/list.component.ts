import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startSearch } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachine } from '@stores/statemachines/models/statemachine.model';
import { merge } from 'rxjs';

@Component({
  selector: 'list-statemachines',
  templateUrl: './list.component.html'
})
export class ListStateMachinesComponent implements OnInit {
  displayedColumns: string[] = ['name', 'description', 'version', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  stateMachines: StateMachine[] = [];
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectStateMachinesResult)).subscribe((state: SearchResult<StateMachine> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.stateMachines = state.content;
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
