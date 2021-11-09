import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddApiDefComponent = class AddApiDefComponent {
    constructor(translateService, dialogRef) {
        this.translateService = translateService;
        this.dialogRef = dialogRef;
        this.addApiDefFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            path: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addApiDefFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addApiDefFormGroup.value);
    }
};
AddApiDefComponent = __decorate([
    Component({
        selector: 'addapi',
        templateUrl: './add-api.component.html'
    })
], AddApiDefComponent);
export { AddApiDefComponent };
//# sourceMappingURL=add-api.component.js.map