import { __decorate } from "tslib";
import { Component } from '@angular/core';
let LoaderComponent = class LoaderComponent {
    constructor(el) {
        this.el = el;
        this.element = el.nativeElement;
    }
    ngOnInit() {
        document.body.appendChild(this.element);
    }
    ngOnDestroy() {
        this.element.remove();
    }
};
LoaderComponent = __decorate([
    Component({
        selector: 'loader',
        templateUrl: './loader.component.html',
        styleUrls: ['./loader.component.scss']
    })
], LoaderComponent);
export { LoaderComponent };
//# sourceMappingURL=loader.component.js.map