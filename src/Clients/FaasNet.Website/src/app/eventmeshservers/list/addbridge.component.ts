import { Component } from '@angular/core';
import { EventMeshServerResult } from '@stores/eventmeshservers/models/eventmeshserver.model';
import { MatPanelContent } from '../../../components/matpanel/matpanelcontent';

export class AddBridgeData {
  from: EventMeshServerResult | null = null;
  to: EventMeshServerResult | null = null;
}

@Component({
  selector: 'addbridge',
  templateUrl: './addbridge.component.html'
})
export class AddBridgeComponent extends MatPanelContent {
  from: EventMeshServerResult | null = null;
  to: EventMeshServerResult | null = null;

  constructor() {
    super();
  }

  override init(data: any): void {
    this.from = (data as AddBridgeData).from;
    this.to = (data as AddBridgeData).to;
  }

  save() {
    this.onClosed.emit({ save: true });
  }

  cancel() {
    this.onClosed.emit({ save : false });
  }
}
