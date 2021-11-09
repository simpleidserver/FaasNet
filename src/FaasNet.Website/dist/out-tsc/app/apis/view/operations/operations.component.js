import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import { startAddOperation, startGet } from '@stores/apis/actions/api.actions';
import * as fromReducers from '@stores/appstate';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddOperationComponent } from './add-operation.component';
let OperationsApiComponents = class OperationsApiComponents {
    constructor(store, dialog, activatedRoute, translateService, snackBar, actions$) {
        this.store = store;
        this.dialog = dialog;
        this.activatedRoute = activatedRoute;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.actions$ = actions$;
        this.displayedColumns = ['name', 'path', 'createDateTime', 'updateDateTime'];
        this.operations = [];
        this.name = "";
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[ApiDefs] COMPLETE_ADD_APIDEF_OPERATION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('apis.messages.apiOperationAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.refresh();
        });
        this.actions$.pipe(filter((action) => action.type === '[ApiDefs] ERROR_ADD_APIDEF_OPERATION'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('apis.messages.errorAddApiOperation'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.operations = state.operations;
        });
    }
    ngAfterViewInit() {
        if (!this.sort || !this.paginator) {
            return;
        }
        merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
        this.refresh();
    }
    addOperation() {
        const dialogRef = this.dialog.open(AddOperationComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt) {
                return;
            }
            const addOperation = startAddOperation({ funcName: this.name, opName: opt.name, opPath: opt.path });
            this.store.dispatch(addOperation);
        });
    }
    refresh() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        const action = startGet({ funcName: name });
        this.store.dispatch(action);
    }
};
__decorate([
    ViewChild(MatPaginator)
], OperationsApiComponents.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], OperationsApiComponents.prototype, "sort", void 0);
OperationsApiComponents = __decorate([
    Component({
        selector: 'list-apioperations',
        templateUrl: './operations.component.html'
    })
], OperationsApiComponents);
export { OperationsApiComponents };
//# sourceMappingURL=operations.component.js.map