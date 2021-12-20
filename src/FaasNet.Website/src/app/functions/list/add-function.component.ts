import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'addfunction',
  templateUrl: './add-function.component.html'
})
export class AddFunctionComponent{
  addFunctionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl(''),
    image: new FormControl('', [Validators.required]),
    version: new FormControl('', [Validators.required])
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddFunctionComponent>) {
  }

  save() {
    if (!this.addFunctionFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addFunctionFormGroup.value);
  }
}
