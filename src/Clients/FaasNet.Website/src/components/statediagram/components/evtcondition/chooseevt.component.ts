import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineEvent } from '@stores/statemachines/models/statemachine-event.model';
import { EventCondition } from '@stores/statemachines/models/statemachine-switch-state.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class ChooseEvtData {
  evtCondition: EventCondition = new EventCondition();
  events: StateMachineEvent[] = [];
}

@Component({
  selector: 'chooseevt',
  templateUrl: './chooseevt.component.html',
  styleUrls: [
    '../state-editor.component.scss'
  ]
})
export class ChooseEvtComponent extends MatPanelContent {
  displayedColumns: string[] = ['actions', 'name', 'type', 'source'];
  evtCondition: EventCondition = new EventCondition();
  events: MatTableDataSource<StateMachineEvent> = new MatTableDataSource<StateMachineEvent>();

  constructor() {
    super();
  }

  override init(data: any): void {
    let evts = (data as ChooseEvtData).events;
    this.evtCondition = (data as ChooseEvtData).evtCondition;
    this.events.data = evts.filter(e => e.kind === "consumed");
  }

  selectEvent(evt: StateMachineEvent) {
    this.evtCondition.eventRef = evt.name;
  }
}
