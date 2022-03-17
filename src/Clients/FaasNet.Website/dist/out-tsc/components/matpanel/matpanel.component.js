import { __decorate } from "tslib";
import { Component, EventEmitter, Output, ViewChild, ViewContainerRef } from '@angular/core';
let MatPanelComponent = class MatPanelComponent {
    constructor(compFactoryResolver) {
        this.compFactoryResolver = compFactoryResolver;
        this.isDisplayed = false;
        this.container = null;
        this.closed = new EventEmitter();
    }
    ngOnInit() {
    }
    init(type, data) {
        this._type = type;
        this._data = data;
    }
    ngAfterViewInit() {
        if (!this.container) {
            return;
        }
        this.container.clear();
        const factory = this.compFactoryResolver.resolveComponentFactory(this._type);
        const componentRef = this.container.createComponent(factory);
        const component = componentRef.instance;
        component.init(this._data);
        component.onClosed.subscribe((e) => {
            this.closed.emit(e);
        });
        setTimeout(() => {
            this.isDisplayed = true;
        }, 2);
    }
    close() {
        this.closed.emit({});
    }
};
__decorate([
    ViewChild('container', { read: ViewContainerRef })
], MatPanelComponent.prototype, "container", void 0);
__decorate([
    Output()
], MatPanelComponent.prototype, "closed", void 0);
MatPanelComponent = __decorate([
    Component({
        selector: 'mat-panel',
        templateUrl: './matpanel.component.html',
        styleUrls: ['./matpanel.component.scss']
    })
], MatPanelComponent);
export { MatPanelComponent };
//# sourceMappingURL=matpanel.component.js.map