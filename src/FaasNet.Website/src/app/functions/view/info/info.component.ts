import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { act } from '@ngrx/effects';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startDelete, startGet } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'info-function',
  templateUrl: './info.component.html'
})
export class InfoFunctionComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  name: string | undefined;
  function: FunctionResult | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private router: Router) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] COMPLETE_DELETE_FUNCTION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('functions.messages.functionRemoved'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/functions']);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] ERROR_DELETE_FUNCTION'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('functions.messages.errorRemoveFunction'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state: FunctionResult | null) => {
      if (!state) {
        return;
      }

      this.function = state;
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

  delete() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startDelete({ name: name });
    this.store.dispatch(action);
  }

  private refresh() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    this.name = name;
    const action = startGet({ name: name });
    this.store.dispatch(action);
  }
}
