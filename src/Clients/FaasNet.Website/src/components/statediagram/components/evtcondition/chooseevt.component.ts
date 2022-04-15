import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { EventDataFilter } from '@stores/statemachines/models/eventdatafilter.model';
import { StateMachineEvent } from '@stores/statemachines/models/statemachine-event.model';
import { EventCondition } from '@stores/statemachines/models/statemachine-switch-state.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { ExpressionEditorComponent } from '../expressioneditor/expressioneditor.component';

export class ChooseEvtData {
  evtCondition: EventCondition = new EventCondition();
  events: StateMachineEvent[] = [];
}

@Component({
  selector: 'chooseevt',
  templateUrl: './chooseevt.component.html',
  styleUrls: [
    './chooseevt.component.scss',
    '../state-editor.component.scss'
  ]
})
export class ChooseEvtComponent extends MatPanelContent {
  editEventFormGroup: FormGroup = new FormGroup({
    useData: new FormControl(),
    data: new FormControl(),
    toStateData: new FormControl()
  });
  displayedColumns: string[] = ['actions', 'name', 'type', 'source'];
  selectedEvtRef: string = "";
  events: MatTableDataSource<StateMachineEvent> = new MatTableDataSource<StateMachineEvent>();

  constructor(private matDialog: MatDialog) {
    super();
  }

  override init(data: any): void {
    let evts = (data as ChooseEvtData).events;
    this.events.data = evts.filter(e => e.kind === "consumed");
    const evtCondition = (data as ChooseEvtData).evtCondition;
    this.selectedEvtRef = evtCondition.eventRef;
    let useData = 'false';
    if (evtCondition.eventDataFilter) {
      useData = evtCondition.eventDataFilter.useData.toString().toLowerCase();
    }

    this.editEventFormGroup.get('useData')?.setValue(useData);
    this.editEventFormGroup.get('data')?.setValue(evtCondition.eventDataFilter?.data);
    this.editEventFormGroup.get('toStateData')?.setValue(evtCondition.eventDataFilter?.toStateData);
  }

  editResults() {
    const self = this;
    let filter: string = "";
    filter = self.editEventFormGroup.get('data')?.value;
    const dialogRef = self.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      self.editEventFormGroup.get('data')?.setValue(r.filter);
    });
  }

  editToStateData() {
    const self = this;
    let filter: string = "";
    filter = self.editEventFormGroup.get('toStateData')?.value;
    const dialogRef = self.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      self.editEventFormGroup.get('toStateData')?.setValue(r.filter);
    });
  }

  save() {
    const record = new EventCondition();
    record.eventRef = this.selectedEvtRef;
    record.eventDataFilter = new EventDataFilter();
    record.eventDataFilter.useData = this.editEventFormGroup.get('useData')?.value;
    record.eventDataFilter.data = this.editEventFormGroup.get('data')?.value;
    record.eventDataFilter.toStateData = this.editEventFormGroup.get('toStateData')?.value;
    this.onClosed.emit(record);
  }

  selectEvent(evt: StateMachineEvent) {
    this.selectedEvtRef = evt.name;
  }
}
