import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'addvpn',
  templateUrl: './add-vpn.component.html'
})
export class AddVpnComponent {
  addVpnFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required])
  });

  constructor(
    private dialogRef: MatDialogRef<AddVpnComponent>) {
  }

  save() {
    if (!this.addVpnFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addVpnFormGroup.value);
  }
}
