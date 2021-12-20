import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startGetConfiguration, startInvoke } from '@stores/functions/actions/function.actions';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'invoke-function',
  templateUrl: './invoke.component.html'
})
export class InvokeFunctionComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  option: any | undefined;
  form: FormGroup = new FormGroup({});
  inputForm: FormControl = new FormControl();
  output: any = {};

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.form = new FormGroup({});
    this.inputForm = new FormControl();
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] COMPLETE_INVOKE_FUNCTION'))
      .subscribe((e) => {
        this.output = e.content;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Functions] ERROR_INVOKE_FUNCTION'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('functions.messages.errorInvokeFunction'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
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
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startGetConfiguration({ name: name });
    this.store.dispatch(action);
  }

  onSave(evt: any) {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const request = {
      configuration: this.form.value,
      input: JSON.parse(this.inputForm.value)
    };
    const invoke = startInvoke({ name: name, request: request });
    this.store.dispatch(invoke);
  }
}
