import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddVpnComponent = class AddVpnComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.addVpnFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addVpnFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addVpnFormGroup.value);
    }
};
AddVpnComponent = __decorate([
    Component({
        selector: 'addvpn',
        templateUrl: './add-vpn.component.html'
    })
], AddVpnComponent);
export { AddVpnComponent };
//# sourceMappingURL=add-vpn.component.js.map