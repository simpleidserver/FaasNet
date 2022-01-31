import { ApplicationRef, ComponentFactoryResolver, ComponentRef, EmbeddedViewRef, Injectable, Injector } from "@angular/core";
import { MatPanelComponent } from "./matpanel.component";

@Injectable({ providedIn: 'root' })
export class MatPanelService {
  private _componentRef: ComponentRef<MatPanelComponent> | null = null;

  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
    private injector: Injector,
    private appRef: ApplicationRef) {

  }

  open(type: any, data: any) {
    const self = this;
    self._componentRef = this.componentFactoryResolver
      .resolveComponentFactory(MatPanelComponent)
      .create(this.injector);
    const instance = self._componentRef.instance as MatPanelComponent;
    instance.init(type, data);
    this.appRef.attachView(self._componentRef.hostView);
    const domElem = (self._componentRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement;
    document.body.appendChild(domElem);
    instance.closed.subscribe(() => {
      this.close();
    });
  }

  close() {
    const self = this;
    if (self._componentRef) {
      self.appRef.detachView(self._componentRef.hostView);
      self._componentRef.destroy();
    }
  }
}
