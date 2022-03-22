import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { getLatestMessages } from '@stores/vpn/actions/vpn.actions';
import { MessageDefinitionResult } from '@stores/vpn/models/messagedefinition.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class LinkEvtsEditorData {
  vpnName: string = "";
  appDomainId: string = "";
}

@Component({
  selector: 'evteditor',
  templateUrl: './evteditor.component.html',
  styleUrls: [
    '../editor.component.scss'
  ]
})
export class LinkEventsEditorComponent extends MatPanelContent {
  data: LinkEvtsEditorData = new LinkEvtsEditorData();
  selectedMessageDef: MessageDefinitionResult = new MessageDefinitionResult();
  displayedColumns: string[] = ['actions', 'name', 'description', 'version'];
  messages: MatTableDataSource<MessageDefinitionResult> = new MatTableDataSource<MessageDefinitionResult>();

  constructor(private store: Store<fromReducers.AppState>) {
    super();
  }

  override init(data: any) {
    this.data = (data as LinkEvtsEditorData);
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this.messages.data = state;
    });
    this.refresh();
  }

  refresh() {
    const act = getLatestMessages({ name: this.data.vpnName, appDomainId: this.data.appDomainId });
    this.store.dispatch(act);
  }

  selectEvent(evt: MessageDefinitionResult) {
    this.selectedMessageDef = evt;
  }

  save() {
    this.onClosed.emit(this.selectedMessageDef);
  }
}
