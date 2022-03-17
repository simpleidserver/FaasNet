import { __decorate } from "tslib";
import { Component, Input } from '@angular/core';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
let JsonComponent = class JsonComponent {
    constructor() {
        this._stateMachineModel = new StateMachineModel();
        this.json = "";
        this.jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
    }
    get stateMachine() {
        return this._stateMachineModel;
    }
    set stateMachine(value) {
        this._stateMachineModel = value;
        this.json = JSON.stringify(this._stateMachineModel.getJson(), null, "\t");
    }
};
__decorate([
    Input()
], JsonComponent.prototype, "stateMachine", null);
JsonComponent = __decorate([
    Component({
        selector: 'display-state-machine-json',
        templateUrl: './json.component.html',
        styleUrls: [
            './json.component.scss',
        ]
    })
], JsonComponent);
export { JsonComponent };
//# sourceMappingURL=json.component.js.map