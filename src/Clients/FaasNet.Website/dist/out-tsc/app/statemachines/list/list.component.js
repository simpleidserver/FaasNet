import { __decorate } from "tslib";
import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startAddEmpty, startSearch } from '@stores/statemachines/actions/statemachines.actions';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddStateMachineComponent } from './add-statemachine.component';
let ListStateMachinesComponent = class ListStateMachinesComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['name', 'description', 'version', 'createDateTime', 'updateDateTime'];
        this.stateMachines = [];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[StateMachines] COMPLETE_ADD_EMPTY_STATE_MACHINE'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('stateMachines.messages.stateMachineAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.refresh();
        });
        this.actions$.pipe(filter((action) => action.type === '[StateMachines] ERROR_ADD_EMPTY_STATE_MACHINE'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('stateMachines.messages.errorAddStateMachine'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectStateMachinesResult)).subscribe((state) => {
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
    addStateMachine() {
        const dialogRef = this.dialog.open(AddStateMachineComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt) {
                return;
            }
            const addStateMachine = startAddEmpty({ name: opt.name, description: opt.description });
            this.store.dispatch(addStateMachine);
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
], ListStateMachinesComponent.prototype, "paginator", void 0);
__decorate([
    ViewChild(MatSort)
], ListStateMachinesComponent.prototype, "sort", void 0);
ListStateMachinesComponent = __decorate([
    Component({
        selector: 'list-statemachines',
        templateUrl: './list.component.html'
    })
], ListStateMachinesComponent);
export { ListStateMachinesComponent };
//# sourceMappingURL=list.component.js.map