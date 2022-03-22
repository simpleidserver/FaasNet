import { __decorate, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
let AddMessageDefComponent = class AddMessageDefComponent {
    constructor(dialogRef, data) {
        var _a, _b, _c;
        this.dialogRef = dialogRef;
        this.data = data;
        this.addMessageDefFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl('', [Validators.required]),
            jsonSchema: new FormControl('', [Validators.required])
        });
        this.jsonOptions = { theme: 'vs', language: 'json' };
        this.isEditable = false;
        if (data) {
            (_a = this.addMessageDefFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue(data.name);
            (_b = this.addMessageDefFormGroup.get('description')) === null || _b === void 0 ? void 0 : _b.setValue(data.description);
            (_c = this.addMessageDefFormGroup.get('jsonSchema')) === null || _c === void 0 ? void 0 : _c.setValue(data.jsonSchema);
            this.isEditable = true;
        }
    }
    save() {
        if (!this.addMessageDefFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addMessageDefFormGroup.value);
    }
};
AddMessageDefComponent = __decorate([
    Component({
        selector: 'add-messagedef',
        templateUrl: './add-message.component.html'
    }),
    __param(1, Inject(MAT_DIALOG_DATA))
], AddMessageDefComponent);
export { AddMessageDefComponent };
//# sourceMappingURL=add-message.component.js.map