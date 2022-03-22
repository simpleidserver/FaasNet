import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetAllClients } from '@stores/vpn/actions/vpn.actions';
import { ClientResult } from '@stores/vpn/models/client.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class ChooseClientData {
  vpnName: string = "";
  appDomainId: string = "";
}

@Component({
  selector: 'chooseclient',
  templateUrl: './chooseclient.component.html',
  styleUrls: [
    '../editor.component.scss'
  ]
})
export class ChooseClientComponent extends MatPanelContent {
  data: ChooseClientData = new ChooseClientData();
  selectedClient: ClientResult = new ClientResult();
  displayedColumns: string[] = ['actions', 'clientId', 'purposes', 'createDateTime'];
  clients: MatTableDataSource<ClientResult> = new MatTableDataSource<ClientResult>();

  constructor(private store: Store<fromReducers.AppState>) {
    super();
  }

  override init(data: any) {
    this.data = (data as ChooseClientData);
    this.store.pipe(select(fromReducers.selectClientsResult)).subscribe((state: ClientResult[] | null) => {
      if (!state) {
        return;
      }

      this.clients.data = state;
    });
    this.refresh();
  }

  getPurpose(purpose: number) {
    switch (purpose) {
      case 1:
        return 'Subscribe';
      case 2:
        return 'Publish';
    }

    return '';
  }

  refresh() {
    const act = startGetAllClients({ name: this.data.vpnName });
    this.store.dispatch(act);
  }

  selectClient(evt: ClientResult) {
    this.selectedClient = evt;
  }

  save() {
    this.onClosed.emit(this.selectedClient);
  }
}
