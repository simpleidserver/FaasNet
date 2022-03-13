import { __decorate } from "tslib";
import { Component } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";
import { BaseRenderingComponent } from "../base-rendering.component";
let ArrayRenderingComponent = class ArrayRenderingComponent extends BaseRenderingComponent {
    constructor() {
        super(...arguments);
        this.children = [];
        this.isLoaded = true;
    }
    addParameter(evt) {
        var _a;
        evt.preventDefault();
        const formGroup = new FormGroup({});
        const formArr = (_a = this.form) === null || _a === void 0 ? void 0 : _a.get(this.option.Name);
        formArr.push(formGroup);
        this.children.push({
            form: formGroup,
            parameters: this.option.Parameters
        });
    }
    removeParameter(evt, child) {
        var _a;
        evt.preventDefault();
        const formArr = (_a = this.form) === null || _a === void 0 ? void 0 : _a.get(this.option.Name);
        const index = this.children.indexOf(child);
        formArr.removeAt(formArr.controls.indexOf(child.form));
        this.children.splice(index, 1);
    }
    setForm(form) {
        var _a;
        if (!form) {
            return;
        }
        this.form = form;
        if (!((_a = this.form) === null || _a === void 0 ? void 0 : _a.contains(this.option.Name))) {
            const formArr = new FormArray([]);
            this.form.addControl(this.option.Name, formArr);
        }
        else {
            const formArr = this.form.controls[this.option.Name];
            formArr.controls.forEach((r) => {
                this.children.push({
                    form: r,
                    parameters: this.option.Parameters
                });
            });
        }
    }
};
ArrayRenderingComponent = __decorate([
    Component({
        selector: 'view-array',
        templateUrl: 'array-rendering.component.html',
        styleUrls: ['./array-rendering.component.scss']
    })
], ArrayRenderingComponent);
export { ArrayRenderingComponent };
//# sourceMappingURL=array-rendering.component.js.map