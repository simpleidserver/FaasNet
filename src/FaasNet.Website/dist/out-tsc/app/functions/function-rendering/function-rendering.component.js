import { __decorate } from "tslib";
import { Component, Input, ViewChild, ViewContainerRef } from "@angular/core";
import { ArrayRenderingComponent } from "./array/array-rendering.component";
import { StringRenderingComponent } from "./string/string-rendering.component";
let FunctionRenderingComponent = class FunctionRenderingComponent {
    constructor(compFactoryResolver, ref) {
        this.compFactoryResolver = compFactoryResolver;
        this.ref = ref;
        this._dic = {
            'array': ArrayRenderingComponent,
            'string': StringRenderingComponent
        };
        this._form = null;
    }
    get form() {
        if (!this._form) {
            return null;
        }
        return this._form;
    }
    set form(val) {
        if (!val) {
            return;
        }
        this._form = val;
        this.refresh();
    }
    get option() {
        return this._option;
    }
    set option(val) {
        if (!val) {
            return;
        }
        this._option = val;
        this.refresh();
    }
    ngAfterViewInit() {
        this.refresh();
    }
    refresh() {
        if (!this.option || !this.container || !this.form) {
            return;
        }
        this.container.clear();
        const type = this._dic[this.option.Type];
        if (!type) {
            return;
        }
        const factory = this.compFactoryResolver.resolveComponentFactory(type);
        this.componentRef = this.container.createComponent(factory);
        this.baseUIComponent = this.componentRef.instance;
        this.baseUIComponent.option = this.option;
        this.baseUIComponent.setForm(this.form);
        setTimeout(() => {
            this.ref.markForCheck();
        }, 10);
    }
};
__decorate([
    ViewChild('container', { read: ViewContainerRef })
], FunctionRenderingComponent.prototype, "container", void 0);
__decorate([
    Input()
], FunctionRenderingComponent.prototype, "form", null);
__decorate([
    Input()
], FunctionRenderingComponent.prototype, "option", null);
FunctionRenderingComponent = __decorate([
    Component({
        selector: 'function-rendering-component',
        templateUrl: 'function-rendering.component.html',
        styleUrls: ['./function-rendering.component.scss']
    })
], FunctionRenderingComponent);
export { FunctionRenderingComponent };
//# sourceMappingURL=function-rendering.component.js.map