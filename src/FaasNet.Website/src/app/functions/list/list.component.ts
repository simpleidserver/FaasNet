import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startSearch } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { merge } from 'rxjs';

@Component({
  selector: 'list-functions',
  templateUrl: './list.component.html'
})
export class ListFunctionsComponent implements OnInit {
  displayedColumns: string[] = ['name', 'image', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  functions: FunctionResult[] = [];
  length: number | undefined;

  constructor(
    private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
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
