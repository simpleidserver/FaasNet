import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetVpn } from '@stores/vpn/actions/vpn.actions';
import { Subscription } from 'rxjs';

@Component({
  selector: 'view-vpn',
  templateUrl: './view.component.html'
})
export class ViewVpnComponent implements OnInit, OnDestroy {
  name: string | undefined;
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
    this.name = this.activatedRoute.snapshot.params['name'];
    if (this.name) {
      const action = startGetVpn({ name: this.name });
      this.store.dispatch(action);
    }
  }
}
