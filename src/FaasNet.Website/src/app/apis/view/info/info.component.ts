import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import { startGet } from '@stores/apis/actions/api.actions';
import { ApiDefinitionResult } from '@stores/apis/models/apidef.model';
import * as fromReducers from '@stores/appstate';
import { Subscription } from 'rxjs';

@Component({
  selector: 'info-function',
  templateUrl: './info.component.html'
})
export class InfoApiComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  name: string | undefined;
  api: ApiDefinitionResult | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state: ApiDefinitionResult | null) => {
      if (!state) {
        return;
      }

      this.api = state;
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
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    this.name = name;
    const action = startGet({ funcName: name });
    this.store.dispatch(action);
  }
}
