import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
let AddStateMachineComponent = class AddStateMachineComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.addStateMachineFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            description: new FormControl('', [Validators.required])
        });
    }
    save() {
        if (!this.addStateMachineFormGroup.valid) {
            return;
        }
        this.dialogRef.close(this.addStateMachineFormGroup.value);
    }
};
AddStateMachineComponent = __decorate([
    Component({
        selector: 'addstatemachine',
        templateUrl: './add-statemachine.component.html'
    })
], AddStateMachineComponent);
export { AddStateMachineComponent };
//# sourceMappingURL=add-statemachine.component.js.map