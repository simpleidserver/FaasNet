import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetAllClients } from '@stores/vpn/actions/vpn.actions';
import { ClientResult } from '@stores/vpn/models/client.model';

@Component({
  selector: 'clients-vpn',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})
export class ClientsVpnComponent implements OnInit {
  clients: ClientResult[] = [];
  displayedColumns: string[] = ['actions', 'clientId', 'purposes', 'createDateTime'];
  name: string = "";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectClientsResult)).subscribe((state: ClientResult[] | null) => {
      if (!state) {
        return;
      }

      this.clients = state;
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

  removeClient(client: ClientResult) {

  }

  private refresh() {
    this.name = this.activatedRoute.parent?.snapshot.params['name'];
    const act = startGetAllClients({ name: this.name });
    this.store.dispatch(act);
  }
}
