import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startAdd, startSearch } from '@stores/functions/actions/function.actions';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddFunctionComponent } from './add-function.component';
let ListFunctionsComponent = class ListFunctionsComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['name', 'image', 'createDateTime', 'updateDateTime'];
        this.functions = [];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[Functions] COMPLETE_ADD_FUNCTION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('functions.messages.functionAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.refresh();
        });
        this.actions$.pipe(filter((action) => action.type === '[Functions] ERROR_ADD_FUNCTION'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('functions.messages.errorAddFunction'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectFunctionsResult)).subscribe((state) => {
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
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt) {
                return;
            }
            const addFunction = startAdd({ name: opt.name, image: opt.image });
            this.store.dispatch(addFunction);
        });
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
], ListFunctionsComponent.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], ListFunctionsComponent.prototype, "sort", void 0);
ListFunctionsComponent = __decorate([
    Component({
        selector: 'list-functions',
        templateUrl: './list.component.html'
    })
], ListFunctionsComponent);
export { ListFunctionsComponent };
//# sourceMappingURL=list.component.js.map