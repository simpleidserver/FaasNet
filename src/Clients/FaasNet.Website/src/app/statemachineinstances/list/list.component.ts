import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startReactivate, startSearch } from '@stores/statemachineinstances/actions/statemachineinstances.actions';
import { StateMachineInstance } from '@stores/statemachineinstances/models/statemachineinstance.model';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'list-statemachineinstances',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListStateMachineInstanceComponent implements OnInit {
  activeVpn: string = "";
  displayedColumns: string[] = ['actions', 'workflowDefName', 'workflowDefDescription', 'workflowDefVersion', 'status', 'createDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  stateMachineInstances: StateMachineInstance[] = [];
  length: number | undefined;
  isLoading: boolean = false;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    const self = this;
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachineInstances] COMPLETE_REACTIVATE_INSTANCE'))
      .subscribe(() => {
        self.isLoading = false;
        self.snackBar.open(self.translateService.instant('stateMachineInstance.messages.stateMachineInstanceReactivated'), self.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachineInstances] ERROR_REACTIVATE_INSTANCE'))
      .subscribe(() => {
        self.isLoading = false;
        self.snackBar.open(self.translateService.instant('stateMachineInstance.messages.errorReactivateStateMachineInstance'), self.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.store.pipe(select(fromReducers.selectStateMachineInstancesResult)).subscribe((state: SearchResult<StateMachineInstance> | null) => {
      if (!state || !state.content) {
        return;
      }

      self.stateMachineInstances = state.content;
      self.length = state.totalLength;
    });
    self.store.pipe(select(fromReducers.selectActiveVpnResult)).subscribe((vpn: string | null) => {
      if (!vpn) {
        return;
      }

      self.activeVpn = vpn;
      self.refresh();
    });
  }

  ngAfterViewInit() {
    if (!this.sort || !this.paginator) {
      return;
    }

    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  reactivate(id: string) {
    this.isLoading = true;
    const action = startReactivate({ id: id });
    this.store.dispatch(action);
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

    let request = startSearch({ order: active, direction, count, startIndex, vpn: this.activeVpn });
    this.store.dispatch(request);
  }
}
