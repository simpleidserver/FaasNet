import { ChangeDetectorRef, Component, ComponentFactoryResolver, ComponentRef, Input, ViewChild, ViewContainerRef } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { ArrayRenderingComponent } from "./array/array-rendering.component";
import { BaseRenderingComponent } from "./base-rendering.component";
import { StringRenderingComponent } from "./string/string-rendering.component";

@Component({
  selector: 'function-rendering-component',
  templateUrl: 'function-rendering.component.html',
  styleUrls: ['./function-rendering.component.scss']
})
export class FunctionRenderingComponent {
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef | undefined;
  private componentRef: ComponentRef<unknown> | undefined;
  private baseUIComponent: BaseRenderingComponent | undefined;
  private _option: any;
  private _dic: any = {
    'array': ArrayRenderingComponent,
    'string': StringRenderingComponent
  };
  private _form: FormGroup | null = null;

  constructor(
    private compFactoryResolver: ComponentFactoryResolver,
    private ref: ChangeDetectorRef) { }

  @Input()
  get form() : FormGroup | null {
    if (!this._form) {
      return null;
    }

    return this._form;
  }
  set form(val: FormGroup | null) {
    if (!val) {
      return;
    }

    this._form = val;
    this.refresh();
  }

  @Input()
  get option() {
    return this._option;
  }
  set option(val: any) {
    if (!val) {
      return;
    }

    this._option = val;
    this.refresh();
  }

  ngAfterViewInit() {
    this.refresh();
  }

  private refresh() {
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
    this.baseUIComponent = this.componentRef.instance as BaseRenderingComponent;
    this.baseUIComponent.option = this.option;
    this.baseUIComponent.setForm(this.form);
    setTimeout(() => {
      this.ref.markForCheck();
    }, 10);
  }
}
