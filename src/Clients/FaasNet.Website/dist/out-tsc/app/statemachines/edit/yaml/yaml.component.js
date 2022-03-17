import { __decorate } from "tslib";
import { Component, Input } from '@angular/core';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { Document } from 'yaml';
let YamlComponent = class YamlComponent {
    constructor() {
        this._stateMachineModel = new StateMachineModel();
        this.yaml = "";
        this.yamlOptions = { theme: 'vs', language: 'yaml' };
    }
    get stateMachine() {
        return this._stateMachineModel;
    }
    set stateMachine(value) {
        this._stateMachineModel = value;
        const doc = new Document();
        doc.contents = this._stateMachineModel.getJson();
        this.yaml = doc.toString();
    }
};
__decorate([
    Input()
], YamlComponent.prototype, "stateMachine", null);
YamlComponent = __decorate([
    Component({
        selector: 'display-state-machine-yaml',
        templateUrl: './yaml.component.html',
        styleUrls: [
            './yaml.component.scss',
        ]
    })
], YamlComponent);
export { YamlComponent };
//# sourceMappingURL=yaml.component.js.map