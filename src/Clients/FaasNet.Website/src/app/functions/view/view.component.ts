import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startGet } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'view-function',
  templateUrl: './view.component.html'
})
export class ViewFunctionComponent implements OnInit, OnDestroy {
  name: string | undefined;
  id: string | undefined;
  subscription: Subscription | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state: FunctionResult | null) => {
      if (!state) {
        return;
      }

      this.name = state.name;
      this.id = state.id;
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
    const action = startGet({ name: name });
    this.store.dispatch(action);
  }
}
