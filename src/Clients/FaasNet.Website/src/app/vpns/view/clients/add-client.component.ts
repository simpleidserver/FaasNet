import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'add-client',
  templateUrl: './add-client.component.html'
})
export class AddClientComponent {
  addClientFormGroup: FormGroup = new FormGroup({
    clientId  : new FormControl('', [Validators.required]),
    purposes: new FormControl('', [Validators.required])
  });

  constructor(
    private dialogRef: MatDialogRef<AddClientComponent>) {
  }

  save() {
    if (!this.addClientFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addClientFormGroup.value);
  }
}
