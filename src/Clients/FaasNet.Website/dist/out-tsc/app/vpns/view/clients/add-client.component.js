import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddClientComponent = class AddClientComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.addClientFormGroup = new FormGroup({
            clientId: new FormControl('', [Validators.required]),
            purposes: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addClientFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addClientFormGroup.value);
    }
};
AddClientComponent = __decorate([
    Component({
        selector: 'add-client',
        templateUrl: './add-client.component.html'
    })
], AddClientComponent);
export { AddClientComponent };
//# sourceMappingURL=add-client.component.js.map