import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'addapplicationdomain',
  templateUrl: './add-applicationdomain.component.html'
})
export class AddApplicationDomainComponent {
  addApplicationDomainFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    rootTopic: new FormControl('', [Validators.required])
  });

  constructor(
    private dialogRef: MatDialogRef<AddApplicationDomainComponent>) {
  }

  save() {
    if (!this.addApplicationDomainFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addApplicationDomainFormGroup.value);
  }
}
