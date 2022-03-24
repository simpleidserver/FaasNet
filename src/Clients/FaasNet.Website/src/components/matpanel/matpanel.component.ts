import { Component, ComponentFactoryResolver, EventEmitter, OnInit, Output, ViewChild, ViewContainerRef } from '@angular/core';
import { MatPanelContent } from './matpanelcontent';

@Component({
  selector: 'mat-panel',
  templateUrl: './matpanel.component.html',
  styleUrls: ['./matpanel.component.scss']
})
export class MatPanelComponent implements OnInit {
  private _type: any;
  private _data: any;
  isDisplayed: boolean = false;
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef | null = null;
  @Output() closed: EventEmitter<any> = new EventEmitter();

  constructor(
    private compFactoryResolver: ComponentFactoryResolver) {
  }

  ngOnInit(): void {

  }

  public init(type: any, data: any) {
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
    const component = componentRef.instance as MatPanelContent;
    component.init(this._data);
    component.onClosed.subscribe((e) => {
      this.closed.emit(e);
    });
    setTimeout(() => {
      this.isDisplayed = true;
    }, 2);
  }

  public close() {
    this.closed.emit(null);
  }
}
