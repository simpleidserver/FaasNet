import { __decorate } from "tslib";
import { Component, EventEmitter, Output } from "@angular/core";
let DefaultConditionComponent = class DefaultConditionComponent {
    constructor() {
        this.closed = new EventEmitter();
    }
    close() {
        this.closed.emit();
    }
};
__decorate([
    Output()
], DefaultConditionComponent.prototype, "closed", void 0);
DefaultConditionComponent = __decorate([
    Component({
        selector: 'defaultcondition-editor',
        templateUrl: './defaultcondition.component.html',
        styleUrls: ['../state-editor.component.scss']
    })
], DefaultConditionComponent);
export { DefaultConditionComponent };
//# sourceMappingURL=defaultcondition.component.js.map