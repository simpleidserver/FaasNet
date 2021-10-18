import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { SearchResult } from '@stores/common/search.model';
import { startSearch } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { merge } from 'rxjs';

class UIFunctionResult extends FunctionResult {
  constructor(fn: FunctionResult) {
    super();
    this.createDateTime = fn.createDateTime;
    this.image = fn.image;
    this.name = fn.name;
    this.updateDateTime = fn.updateDateTime;
    this.isSelected = false;
  }

  isSelected: boolean;
}

@Component({
  selector: 'addapifunction',
  templateUrl: './add-function.component.html'
})
export class AddFunctionComponent implements OnInit {
  displayedColumns: string[] = ['action', 'name', 'image', 'createDateTime', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;
  functions: UIFunctionResult[] = [];
  length: number | undefined;

  constructor(
    private dialogRef: MatDialogRef<AddFunctionComponent>,
    private store: Store<fromReducers.AppState>) {
  }

  ngOnInit() {
    this.store.pipe(select(fromReducers.selectFunctionsResult)).subscribe((state: SearchResult<FunctionResult> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.functions = state.content.map(s => new UIFunctionResult(s));
      this.length = state.totalLength;
    });
    this.refresh();
  }

  save() {
    let selectedFunction: UIFunctionResult | null = null;
    const filtered = this.functions.filter(f => f.isSelected);
    if (filtered.length === 1) {
      selectedFunction = filtered[0];
    }

    this.dialogRef.close(selectedFunction);
  }

  isFunctionSelected() {
    return this.functions.filter(f => f.isSelected).length === 1;
  }

  toggle(fn: UIFunctionResult) {
    this.functions.forEach(f => {
      if (f.name !== fn.name) {
        f.isSelected = false;
      }
    });

    fn.isSelected = !fn.isSelected;
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
