import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddOperationComponent = class AddOperationComponent {
    constructor(translateService, dialogRef) {
        this.translateService = translateService;
        this.dialogRef = dialogRef;
        this.addOperationFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            path: new FormControl('')
        });
    }
    save() {
        if (!this.addOperationFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addOperationFormGroup.value);
    }
};
AddOperationComponent = __decorate([
    Component({
        selector: 'addoperation',
        templateUrl: './add-operation.component.html'
    })
], AddOperationComponent);
export { AddOperationComponent };
//# sourceMappingURL=add-operation.component.js.map