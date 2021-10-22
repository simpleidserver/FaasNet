import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startInvokeOperation } from '@stores/apis/actions/api.actions';

export class LaunchFunctionData {
  funcName: string = "";
  opName: string = "";
}

@Component({
  selector: 'launch-function',
  templateUrl: './launch-function-dialog.component.html'
})
export class LaunchFunctionDialogComponent implements OnInit, OnDestroy {
  launchFunctionFormGroup: FormGroup = new FormGroup({
    input: new FormControl()
  });
  subscription: any | undefined;
  isLoading: boolean = false;
  result: any;

  constructor(
    private dialogRef: MatDialogRef<LaunchFunctionDialogComponent>,
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: LaunchFunctionData) {
  }

  ngOnInit() {
    this.subscription = this.store.pipe(select(fromReducers.selectApiOperationInvocationResult)).subscribe((state: any | null) => {
      if (!state || !this.isLoading) {
        return;
      }

      this.isLoading = false;
      this.result = state;
    });
  }

  ngOnDestroy() {
    if (!this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  launch() {
    if (!this.launchFunctionFormGroup.valid) {
      return;
    }

    this.isLoading = true;
    const value = this.launchFunctionFormGroup.value;
    const startGet = startInvokeOperation({ funcName: this.data.funcName, opName: this.data.opName, request: JSON.parse(value.input) });
    this.store.dispatch(startGet);
  }
}
