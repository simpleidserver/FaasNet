import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetConfiguration } from '@stores/functions/actions/function.actions';
import { Subscription } from 'rxjs';

@Component({
  selector: 'view-function',
  templateUrl: './view.component.html'
})
export class ViewFunctionComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  name: string | undefined;
  option: any | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute  ) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectFunctionConfigurationResult)).subscribe((state: any | null) => {
      if (!state) {
        return;
      }

      this.option = state;
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
    const name = this.activatedRoute.snapshot.params['name'];
    this.name = name;
    const action = startGetConfiguration({ name: name });
    this.store.dispatch(action);
  }
}
