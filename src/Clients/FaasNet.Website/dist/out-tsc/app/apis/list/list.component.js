import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import { startAdd, startSearch } from '@stores/apis/actions/api.actions';
import * as fromReducers from '@stores/appstate';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddApiDefComponent } from './add-api.component';
let ListApiDefComponent = class ListApiDefComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['name', 'path', 'nbOperations', 'createDateTime', 'updateDateTime'];
        this.apiDefs = [];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[ApiDefs] COMPLETE_ADD_APIDEF'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('apis.messages.apiAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.refresh();
        });
        this.actions$.pipe(filter((action) => action.type === '[Functions] ERROR_ADD_APIDEF'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('apis.messages.errorAddApi'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectApiDefsResult)).subscribe((state) => {
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
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt) {
                return;
            }
            const addFunction = startAdd({ name: opt.name, path: opt.path });
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
], ListApiDefComponent.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], ListApiDefComponent.prototype, "sort", void 0);
ListApiDefComponent = __decorate([
    Component({
        selector: 'list-apidefs',
        templateUrl: './list.component.html'
    })
], ListApiDefComponent);
export { ListApiDefComponent };
//# sourceMappingURL=list.component.js.map