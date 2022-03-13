import { __decorate, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetConfiguration } from '@stores/functions/actions/function.actions';
let UpdateFunctionConfigurationComponent = class UpdateFunctionConfigurationComponent {
    constructor(dialogRef, store, data) {
        this.dialogRef = dialogRef;
        this.store = store;
        this.data = data;
        this.form = new FormGroup({});
        this.extractForm(data.configuration, this.form);
        this.refresh();
    }
    ngOnInit() {
        this.sub = this.store.pipe(select(fromReducers.selectFunctionConfigurationResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.option = state;
        });
    }
    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
    ngAfterViewInit() {
        this.refresh();
    }
    save() {
        if (!this.form.valid) {
            return;
        }
        this.dialogRef.close(this.form.value);
    }
    extractForm(data, form) {
        if (!this.data) {
            return;
        }
        for (var key in data) {
            const record = data[key];
            if (Array.isArray(record)) {
                const formArr = new FormArray([]);
                record.forEach((r) => {
                    const formGroup = new FormGroup({});
                    this.extractForm(r, formGroup);
                    formArr.push(formGroup);
                });
                form.setControl(key, formArr);
            }
            else {
                const formControl = new FormControl();
                formControl.setValue(record);
                form.setControl(key, formControl);
            }
        }
    }
    refresh() {
        if (!this.data || !this.data.info || !this.data.info.name) {
            return;
        }
        let request = startGetConfiguration({ name: this.data.info.name });
        this.store.dispatch(request);
    }
};
UpdateFunctionConfigurationComponent = __decorate([
    Component({
        selector: 'updatefunctionconfiguration',
        templateUrl: './update-configuration.component.html',
        styleUrls: ['./update-configuration.component.scss']
    }),
    __param(2, Inject(MAT_DIALOG_DATA))
], UpdateFunctionConfigurationComponent);
export { UpdateFunctionConfigurationComponent };
//# sourceMappingURL=update-configuration.component.js.map