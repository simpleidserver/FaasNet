import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class ChooseMessageData {
  vpnName: string = "";
  appDomainId: string = "";
  messageId: string | null = null;
}

@Component({
  selector: 'choosemessage',
  templateUrl: './choosemessage.component.html',
  styleUrls: [
    '../editor.component.scss'
  ]
})
export class ChooseMessageComponent extends MatPanelContent {
  data: ChooseMessageData = new ChooseMessageData();
  selectedMessageDef: MessageDefinitionResult = new MessageDefinitionResult();
  displayedColumns: string[] = ['actions', 'name', 'description', 'version'];
  messages: MatTableDataSource<MessageDefinitionResult> = new MatTableDataSource<MessageDefinitionResult>();

  constructor(private store: Store<fromReducers.AppState>) {
    super();
  }

  override init(data: any) {
    this.data = (data as ChooseMessageData);
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this.messages.data = state;
      if (this.data.messageId) {
        this.selectedMessageDef = this.messages.data.filter((d) => d.id === this.data.messageId)[0];
      }
    });
  }

  selectEvent(evt: MessageDefinitionResult) {
    this.selectedMessageDef = evt;
  }

  save() {
    this.onClosed.emit(this.selectedMessageDef);
  }
}
