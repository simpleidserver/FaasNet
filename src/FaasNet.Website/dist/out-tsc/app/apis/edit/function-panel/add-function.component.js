import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startSearch } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { merge } from 'rxjs';
class UIFunctionResult extends FunctionResult {
    constructor(fn) {
        super();
        this.createDateTime = fn.createDateTime;
        this.image = fn.image;
        this.name = fn.name;
        this.updateDateTime = fn.updateDateTime;
        this.isSelected = false;
    }
}
let AddFunctionComponent = class AddFunctionComponent {
    constructor(dialogRef, store) {
        this.dialogRef = dialogRef;
        this.store = store;
        this.displayedColumns = ['action', 'name', 'image', 'createDateTime', 'updateDateTime'];
        this.functions = [];
    }
    ngOnInit() {
        this.store.pipe(select(fromReducers.selectFunctionsResult)).subscribe((state) => {
            if (!state || !state.content) {
                return;
            }
            this.functions = state.content.map(s => new UIFunctionResult(s));
            this.length = state.totalLength;
        });
        this.refresh();
    }
    save() {
        let selectedFunction = null;
        const filtered = this.functions.filter(f => f.isSelected);
        if (filtered.length === 1) {
            selectedFunction = filtered[0];
        }
        this.dialogRef.close(selectedFunction);
    }
    isFunctionSelected() {
        return this.functions.filter(f => f.isSelected).length === 1;
    }
    toggle(fn) {
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
], AddFunctionComponent.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], AddFunctionComponent.prototype, "sort", void 0);
AddFunctionComponent = __decorate([
    Component({
        selector: 'addapifunction',
        templateUrl: './add-function.component.html'
    })
], AddFunctionComponent);
export { AddFunctionComponent };
//# sourceMappingURL=add-function.component.js.map