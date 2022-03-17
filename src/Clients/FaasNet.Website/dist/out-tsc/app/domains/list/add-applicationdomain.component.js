import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddApplicationDomainComponent = class AddApplicationDomainComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.addApplicationDomainFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl('', [Validators.required]),
            rootTopic: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addApplicationDomainFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addApplicationDomainFormGroup.value);
    }
};
AddApplicationDomainComponent = __decorate([
    Component({
        selector: 'addapplicationdomain',
        templateUrl: './add-applicationdomain.component.html'
    })
], AddApplicationDomainComponent);
export { AddApplicationDomainComponent };
//# sourceMappingURL=add-applicationdomain.component.js.map