import { __decorate } from "tslib";
import { Injectable } from "@angular/core";
import { MatPanelComponent } from "./matpanel.component";
let MatPanelService = class MatPanelService {
    constructor(componentFactoryResolver, injector, appRef) {
        this.componentFactoryResolver = componentFactoryResolver;
        this.injector = injector;
        this.appRef = appRef;
        this._componentRef = null;
    }
    open(type, data) {
        const self = this;
        self._componentRef = this.componentFactoryResolver
            .resolveComponentFactory(MatPanelComponent)
            .create(this.injector);
        const instance = self._componentRef.instance;
        instance.init(type, data);
        this.appRef.attachView(self._componentRef.hostView);
        const domElem = self._componentRef.hostView.rootNodes[0];
        document.body.appendChild(domElem);
        instance.closed.subscribe(() => {
            this.close();
        });
        return instance;
    }
    close() {
        const self = this;
        if (self._componentRef) {
            self.appRef.detachView(self._componentRef.hostView);
            self._componentRef.destroy();
        }
    }
};
MatPanelService = __decorate([
    Injectable({ providedIn: 'root' })
], MatPanelService);
export { MatPanelService };
//# sourceMappingURL=matpanelservice.js.map