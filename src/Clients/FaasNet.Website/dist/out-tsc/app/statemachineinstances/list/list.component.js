import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startSearch } from '@stores/statemachineinstances/actions/statemachineinstances.actions';
import { merge } from 'rxjs';
let ListStateMachineInstanceComponent = class ListStateMachineInstanceComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['workflowDefName', 'workflowDefDescription', 'workflowDefVersion', 'status', 'createDateTime'];
        this.stateMachineInstances = [];
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectStateMachineInstancesResult)).subscribe((state) => {
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
    refresh() {
        if (!this.paginator || !this.sort) {
            return;
        }
        let startIndex = 0;
        let count = 5;
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
};
__decorate([
    ViewChild(MatPaginator)
], ListStateMachineInstanceComponent.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], ListStateMachineInstanceComponent.prototype, "sort", void 0);
ListStateMachineInstanceComponent = __decorate([
    Component({
        selector: 'list-statemachineinstances',
        templateUrl: './list.component.html'
    })
], ListStateMachineInstanceComponent);
export { ListStateMachineInstanceComponent };
//# sourceMappingURL=list.component.js.map