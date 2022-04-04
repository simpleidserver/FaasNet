import { Component, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { StateMachineEvent } from '@stores/statemachines/models/statemachine-event.model';
import { MatDialog } from '@angular/material/dialog';
import { EditEventDialogComponent } from './editevent-dialog.component';

@Component({
  selector: 'eventseditor-statemachine',
  templateUrl: './eventseditor.component.html'
})
export class StateMachineEventsEditorComponent {
  displayedColumns: string[] = ['actions', 'name', 'type', 'source', 'kind'];
  evts: MatTableDataSource<StateMachineEvent> = new MatTableDataSource<StateMachineEvent>();
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    if (value) {
      this.evts.data = value.events;
    }
  }

  get vpn() {
    return this._stateMachineModel.vpn;
  }

  get applicationDomainId() {
    return this._stateMachineModel.applicationDomainId;
  }

  constructor(private dialog: MatDialog) {
  }

  editEvent(evt: StateMachineEvent, index: number) {
    const dialogRef = this.dialog.open(EditEventDialogComponent, {
      data: evt,
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((d) => {
      if (!d) {
        return;
      }

      this.evts.data.splice(index, 1);
      this.evts.data.push(d);
      this.evts.data = this.evts.data;
    });
  }
}
