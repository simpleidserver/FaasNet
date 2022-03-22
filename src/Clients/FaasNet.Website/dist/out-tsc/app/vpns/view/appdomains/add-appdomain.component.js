import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddAppDomainComponent = class AddAppDomainComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.addAppDomainFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl('', [Validators.required]),
            rootTopic: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addAppDomainFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addAppDomainFormGroup.value);
    }
};
AddAppDomainComponent = __decorate([
    Component({
        selector: 'add-appdomain',
        templateUrl: './add-appdomain.component.html'
    })
], AddAppDomainComponent);
export { AddAppDomainComponent };
//# sourceMappingURL=add-appdomain.component.js.map