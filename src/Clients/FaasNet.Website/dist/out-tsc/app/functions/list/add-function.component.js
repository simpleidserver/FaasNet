import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddFunctionComponent = class AddFunctionComponent {
    constructor(translateService, dialogRef) {
        this.translateService = translateService;
        this.dialogRef = dialogRef;
        this.addFunctionFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl(''),
            image: new FormControl('', [Validators.required]),
            version: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addFunctionFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addFunctionFormGroup.value);
    }
};
AddFunctionComponent = __decorate([
    Component({
        selector: 'addfunction',
        templateUrl: './add-function.component.html'
    })
], AddFunctionComponent);
export { AddFunctionComponent };
//# sourceMappingURL=add-function.component.js.map