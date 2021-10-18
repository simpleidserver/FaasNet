import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetConfiguration } from '@stores/functions/actions/function.actions';
import { FunctionRecord } from './function.model';

@Component({
  selector: 'updatefunctionconfiguration',
  templateUrl: './update-configuration.component.html',
  styleUrls: ['./update-configuration.component.scss']
})
export class UpdateFunctionConfigurationComponent implements OnInit, OnDestroy {
  sub: any;
  option: any;
  form: FormGroup = new FormGroup({});

  constructor(
    private dialogRef: MatDialogRef<UpdateFunctionConfigurationComponent>,
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: FunctionRecord) {
    this.extractForm(data.configuration, this.form);
    this.refresh();
  }

  ngOnInit() {
    this.sub = this.store.pipe(select(fromReducers.selectFunctionConfigurationResult)).subscribe((state: any) => {
      if (!state) {
        return;
      }

      this.option = state;
    });
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  ngAfterViewInit() {
    this.refresh();
  }

  save() {
    if (!this.form.valid) {
      return;
    }

    this.dialogRef.close(this.form.value);
  }

  private extractForm(data: any, form: FormGroup) {
    if (!this.data) {
      return;
    }

    for (var key in data) {
      const record = data[key];
      if (Array.isArray(record)) {
        const formArr = new FormArray([]);
        record.forEach((r: any) => {
          const formGroup = new FormGroup({});
          this.extractForm(r, formGroup);
          formArr.push(formGroup);
        });
        form.setControl(key, formArr);
      } else {
        const formControl = new FormControl();
        formControl.setValue(record);
        form.setControl(key, formControl);
      }
    }
  }

  private refresh() {
    if (!this.data || !this.data.info || !this.data.info.name) {
      return;
    }

    let request = startGetConfiguration({ name: this.data.info.name });
    this.store.dispatch(request);
  }
}
