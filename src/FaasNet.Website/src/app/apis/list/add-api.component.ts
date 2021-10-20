import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'addapi',
  templateUrl: './add-api.component.html'
})
export class AddApiDefComponent{
  addApiDefFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    path: new FormControl('', [Validators.required])
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddApiDefComponent>) {
  }

  save() {
    if (!this.addApiDefFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addApiDefFormGroup.value);
  }
}
