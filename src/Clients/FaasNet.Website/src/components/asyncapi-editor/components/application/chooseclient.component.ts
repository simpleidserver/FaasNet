import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetAllClients } from '@stores/clients/actions/clients.actions';
import { ClientResult } from '@stores/clients/models/client.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class ChooseClientData {
  vpnName: string = "";
  appDomainId: string = "";
  clientId: string | null = null;
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
      if (this.data.clientId) {
        this.selectedClient = this.clients.data.filter((c) => c.clientId === this.data.clientId)[0];
      }
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
    const act = startGetAllClients({ vpn: this.data.vpnName });
    this.store.dispatch(act);
  }

  selectClient(evt: ClientResult) {
    this.selectedClient = evt;
  }

  save() {
    this.onClosed.emit(this.selectedClient);
  }
}
