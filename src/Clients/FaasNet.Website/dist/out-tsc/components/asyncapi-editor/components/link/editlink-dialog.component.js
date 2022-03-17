import { __decorate, __param } from "tslib";
import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
function jsonValidator(control) {
    try {
        if (!control.value) {
            return null;
        }
        JSON.parse(control.value);
    }
    catch (e) {
        return { jsonInvalid: true };
    }
    return null;
}
;
let EditLinkDialogComponent = class EditLinkDialogComponent {
    constructor(data, dialogRef) {
        this.data = data;
        this.dialogRef = dialogRef;
        this.jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
        this.editLinkFormGroup = new FormGroup({
            name: new FormControl('', Validators.required),
            payload: new FormControl('', [Validators.required, jsonValidator])
        });
        this._init(data);
    }
    save() {
        if (!this.editLinkFormGroup.valid) {
            return;
        }
        const val = this.editLinkFormGroup.value;
        this.dialogRef.close({
            name: val.name,
            payload: JSON.parse(val.payload)
        });
    }
    _init(data) {
        var _a, _b;
        if (!data) {
            return;
        }
        (_a = this.editLinkFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue(data.name);
        (_b = this.editLinkFormGroup.get('payload')) === null || _b === void 0 ? void 0 : _b.setValue(JSON.stringify(data.payload, null, "\t"));
    }
};
EditLinkDialogComponent = __decorate([
    Component({
        selector: 'edit-link-dialog-dialog',
        templateUrl: './editlink-dialog.component.html'
    }),
    __param(0, Inject(MAT_DIALOG_DATA))
], EditLinkDialogComponent);
export { EditLinkDialogComponent };
//# sourceMappingURL=editlink-dialog.component.js.map