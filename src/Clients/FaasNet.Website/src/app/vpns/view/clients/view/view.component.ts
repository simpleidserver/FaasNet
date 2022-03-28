import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetClient } from '@stores/clients/actions/clients.actions';
import { Subscription } from 'rxjs';
import { ClientResult } from '@stores/clients/models/client.model';

@Component({
  selector: 'view-vpn-client',
  templateUrl: './view.component.html'
})
export class ViewVpnClientComponent implements OnInit, OnDestroy {
  vpnName: string = "";
  clientId: string = "";
  subscription: Subscription | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectClientResult)).subscribe((state: ClientResult | null) => {
      if (!state) {
        return;
      }

      this.clientId = state.clientId;
    });
    this.subscription = this.activatedRoute.params.subscribe(() => {
      this.refresh();
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  private refresh() {
    const params = this.activatedRoute.snapshot.params;
    this.vpnName = params['vpnName'];
    const act = startGetClient({ id: params['clientId'] });
    this.store.dispatch(act);
  }
}
