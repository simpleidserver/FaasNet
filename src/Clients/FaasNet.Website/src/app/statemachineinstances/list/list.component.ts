import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startSearch } from '@stores/statemachineinstances/actions/statemachineinstances.actions';
import { StateMachineInstance } from '@stores/statemachineinstances/models/statemachineinstance.model';
import { merge } from 'rxjs';

@Component({
  selector: 'list-statemachineinstances',
  templateUrl: './list.component.html'
})
export class ListStateMachineInstanceComponent implements OnInit {
  displayedColumns: string[] = ['workflowDefName', 'workflowDefDescription', 'workflowDefVersion', 'status', 'createDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  stateMachineInstances: StateMachineInstance[] = [];
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectStateMachineInstancesResult)).subscribe((state: SearchResult<StateMachineInstance> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.stateMachineInstances = state.content;
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
