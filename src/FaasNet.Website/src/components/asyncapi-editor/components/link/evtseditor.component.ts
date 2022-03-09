import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { Message } from '../../models/message';
import { EditLinkDialogComponent } from './editlink-dialog.component';

export class LinkEvtsEditorData {
  evts: Message[] = [];
}

@Component({
  selector: 'evtseditor',
  templateUrl: './evtseditor.component.html',
  styleUrls: [
    './evtseditor.component.scss',
    '../editor.component.scss'
  ]
})
export class LinkEventsEditorComponent extends MatPanelContent {
  displayedColumns: string[] = ['actions', 'name', 'title', 'description'];
  evts: MatTableDataSource<Message> = new MatTableDataSource<Message>();
  functions: StateMachineFunction[] = [];

  constructor(
    private matDialog: MatDialog) {
    super();
  }

  override init(data: any) {
    this.evts.data = (data as LinkEvtsEditorData).evts;
  }

  addEvent() {
    const dialogRef = this.matDialog.open(EditLinkDialogComponent, {
      width: '600px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      this.evts.data.push(e);
      this.evts.data = this.evts.data;
    });
  }

  removeEvent(i: number) {
    this.evts.data.splice(i, 1);
    this.evts.data = this.evts.data;
  }

  editEvent(msg: Message, i: number) {
    const dialogRef = this.matDialog.open(EditLinkDialogComponent, {
      data: msg,
      width: '600px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const existingMessage = this.evts.data[i];
      existingMessage.name = e.name;
      existingMessage.payload = e.payload;
      this.evts.data = this.evts.data;
    });
  }

  save() {
    this.onClosed.emit(this.evts.data);
  }
}
