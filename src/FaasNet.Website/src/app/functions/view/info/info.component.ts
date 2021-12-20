import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startDelete } from '@stores/functions/actions/function.actions';
import { FunctionResult } from '@stores/functions/models/function.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'info-function',
  templateUrl: './info.component.html'
})
export class InfoFunctionComponent implements OnInit, OnDestroy {
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
  }

  ngOnDestroy() {
  }

  delete() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startDelete({ name: name });
    this.store.dispatch(action);
  }
}
