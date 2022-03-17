import { __decorate } from "tslib";
import { Component } from '@angular/core';
let LaunchStateMachineComponent = class LaunchStateMachineComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.json = null;
        this.parameters = null;
        this.jsonEditorOptions = { theme: 'vs', language: 'json' };
    }
    launch() {
        this.dialogRef.close({ json: this.json, parameters: this.parameters });
    }
};
LaunchStateMachineComponent = __decorate([
    Component({
        selector: 'launchstatemachine',
        templateUrl: './launch-statemachine.component.html'
    })
], LaunchStateMachineComponent);
export { LaunchStateMachineComponent };
//# sourceMappingURL=launch-statemachine.component.js.map