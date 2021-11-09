import { __decorate } from "tslib";
import { Component, EventEmitter } from "@angular/core";
let BaseRenderingComponent = class BaseRenderingComponent {
    constructor() {
        this.onInitialized = new EventEmitter();
        this.form = null;
    }
    ngOnInit() {
        this.onInitialized.emit();
    }
    setForm(form) {
    }
};
BaseRenderingComponent = __decorate([
    Component({
        template: ''
    })
], BaseRenderingComponent);
export { BaseRenderingComponent };
//# sourceMappingURL=base-rendering.component.js.map