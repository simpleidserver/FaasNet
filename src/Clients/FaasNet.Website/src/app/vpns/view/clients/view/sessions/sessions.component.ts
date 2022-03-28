import { Component, OnDestroy, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { ClientResult } from '@stores/clients/models/client.model';
import { ClientSessionResult } from '@stores/clients/models/clientsession.model';

@Component({
  selector: 'view-vpn-client-sessions',
  templateUrl: './sessions.component.html'
})
export class ViewVpnClientSessionsComponent implements OnInit, OnDestroy {
  sessions: ClientSessionResult[] = [];
  displayedColumns: string[] = ['purpose', 'bufferCloudEvents', 'topics', 'state', 'createDateTime'];

  constructor(
    private store: Store<fromReducers.AppState>) {

  }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectClientResult)).subscribe((state: ClientResult | null) => {
      if (!state) {
        return;
      }

      this.sessions = state.sessions;
    });
  }

  ngOnDestroy() {
  }
}
