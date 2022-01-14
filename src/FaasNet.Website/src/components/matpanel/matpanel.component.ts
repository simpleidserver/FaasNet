import { Component, ComponentFactoryResolver, ViewChild, ViewContainerRef } from '@angular/core';
import { MatPanelContent } from './matpanelcontent';

@Component({
  selector: 'mat-panel',
  templateUrl: './matpanel.component.html',
  styleUrls: ['./matpanel.component.scss']
})
export class MatPanelComponent {
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef | null = null;
  isDisplayed: boolean = false;

  constructor(private compFactoryResolver: ComponentFactoryResolver) { }

  public open(type: any, data : any) : MatPanelContent | null {
    if (!this.container) {
      return null;
    }

    this.isDisplayed = true;
    this.container.clear();
    const factory = this.compFactoryResolver.resolveComponentFactory(type);
    const component = this.container.createComponent(factory);
    var panelContent = component.instance as MatPanelContent;
    panelContent.init(data);
    panelContent.onClosed.subscribe(() => {
      this.isDisplayed = false;
    });
    return panelContent;
  }

  public close() {
    this.isDisplayed = false;
  }
}
