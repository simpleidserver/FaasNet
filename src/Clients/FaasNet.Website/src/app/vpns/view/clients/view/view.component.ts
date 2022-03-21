import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetClient } from '@stores/vpn/actions/vpn.actions';
import { Subscription } from 'rxjs';

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
    this.clientId = params['clientId'];
    const act = startGetClient({ name: this.vpnName, clientId: this.clientId });
    this.store.dispatch(act);
  }
}
