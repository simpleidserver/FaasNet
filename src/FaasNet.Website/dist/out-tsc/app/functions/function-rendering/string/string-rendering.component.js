import { __decorate } from "tslib";
import { Component } from "@angular/core";
import { FormControl } from "@angular/forms";
import { BaseRenderingComponent } from "../base-rendering.component";
let StringRenderingComponent = class StringRenderingComponent extends BaseRenderingComponent {
    constructor() {
        super(...arguments);
        this.control = new FormControl();
    }
    setForm(form) {
        if (!form) {
            return;
        }
        this.form = form;
        if (!this.form.contains(this.option.Name)) {
            this.form.addControl(this.option.Name, this.control);
        }
        else {
            this.control = this.form.get(this.option.Name);
        }
    }
};
StringRenderingComponent = __decorate([
    Component({
        selector: 'view-string',
        templateUrl: 'string-rendering.component.html',
        styleUrls: ['./string-rendering.component.scss']
    })
], StringRenderingComponent);
export { StringRenderingComponent };
//# sourceMappingURL=string-rendering.component.js.map