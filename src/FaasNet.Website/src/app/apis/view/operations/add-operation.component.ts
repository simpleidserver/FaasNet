import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'addoperation',
  templateUrl: './add-operation.component.html'
})
export class AddOperationComponent{
  addOperationFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    path: new FormControl('')
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddOperationComponent>) {
  }

  save() {
    if (!this.addOperationFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addOperationFormGroup.value);
  }
}
