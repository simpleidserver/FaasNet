import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'addstatemachine',
  templateUrl: './add-statemachine.component.html'
})
export class AddStateMachineComponent {
  addStateMachineFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required])
  });

  constructor(
    private dialogRef: MatDialogRef<AddStateMachineComponent>) {
  }

  save() {
    if (!this.addStateMachineFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addStateMachineFormGroup.value);
  }
}
