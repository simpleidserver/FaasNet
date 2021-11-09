import { __decorate, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startInvokeOperation } from '@stores/apis/actions/api.actions';
export class LaunchFunctionData {
    constructor() {
        this.funcName = "";
        this.opName = "";
    }
}
let LaunchFunctionDialogComponent = class LaunchFunctionDialogComponent {
    constructor(dialogRef, store, data) {
        this.dialogRef = dialogRef;
        this.store = store;
        this.data = data;
        this.launchFunctionFormGroup = new FormGroup({
            input: new FormControl()
        });
        this.isLoading = false;
    }
    ngOnInit() {
        this.subscription = this.store.pipe(select(fromReducers.selectApiOperationInvocationResult)).subscribe((state) => {
            if (!state || !this.isLoading) {
                return;
            }
            this.isLoading = false;
            this.result = state;
        });
    }
    ngOnDestroy() {
        if (!this.subscription) {
            this.subscription.unsubscribe();
        }
    }
    launch() {
        if (!this.launchFunctionFormGroup.valid) {
            return;
        }
        this.isLoading = true;
        const value = this.launchFunctionFormGroup.value;
        const startGet = startInvokeOperation({ funcName: this.data.funcName, opName: this.data.opName, request: JSON.parse(value.input) });
        this.store.dispatch(startGet);
    }
};
LaunchFunctionDialogComponent = __decorate([
    Component({
        selector: 'launch-function',
        templateUrl: './launch-function-dialog.component.html'
    }),
    __param(2, Inject(MAT_DIALOG_DATA))
], LaunchFunctionDialogComponent);
export { LaunchFunctionDialogComponent };
//# sourceMappingURL=launch-function-dialog.component.js.map