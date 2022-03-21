import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetAppDomain } from '@stores/vpn/actions/vpn.actions';
import { AppDomainResult } from '@stores/vpn/models/appdomain.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'view-vpn-appdomain',
  templateUrl: './view.component.html'
})
export class ViewVpnAppDomainComponent implements OnInit, OnDestroy {
  vpnName: string = "";
  appDomainId: string = "";
  appDomainName: string = "";
  subscription: Subscription | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectAppDomainResult)).subscribe((state: AppDomainResult | null) => {
      if (!state) {
        return;
      }

      this.appDomainName = state.name;
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
    this.appDomainId = params['appDomainId'];
    const act = startGetAppDomain({ name: this.vpnName, appDomainId: this.appDomainId });
    this.store.dispatch(act);
  }
}
