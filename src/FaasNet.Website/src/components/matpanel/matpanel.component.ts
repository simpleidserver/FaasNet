import { Component, ComponentFactoryResolver, ComponentRef, ViewChild, ViewContainerRef } from '@angular/core';

@Component({
  selector: 'mat-panel',
  templateUrl: './matpanel.component.html',
  styleUrls: ['./matpanel.component.scss']
})
export class MatPanelComponent {
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef | null = null;
  isDisplayed: boolean = false;

  constructor(private compFactoryResolver: ComponentFactoryResolver) { }

  public open(type: any) {
    if (!this.container) {
      return;
    }

    this.isDisplayed = true;
    this.container.clear();
    const factory = this.compFactoryResolver.resolveComponentFactory(type);
    this.container.createComponent(factory);
  }

  public close() {
    this.isDisplayed = false;
  }
}
