import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'add-appdomain',
  templateUrl: './add-appdomain.component.html'
})
export class AddAppDomainComponent {
  addAppDomainFormGroup: FormGroup = new FormGroup({
    name  : new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    rootTopic: new FormControl('', [Validators.required])
  });

  constructor(
    private dialogRef: MatDialogRef<AddAppDomainComponent>) {
  }

  save() {
    if (!this.addAppDomainFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addAppDomainFormGroup.value);
  }
}
