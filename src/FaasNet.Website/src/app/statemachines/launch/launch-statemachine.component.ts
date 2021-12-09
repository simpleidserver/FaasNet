import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'launchstatemachine',
  templateUrl: './launch-statemachine.component.html'
})
export class LaunchStateMachineComponent {
  json: any = null;
  jsonEditorOptions = { theme: 'vs', language: 'json' };

  constructor(
    private dialogRef: MatDialogRef<LaunchStateMachineComponent>) {
  }

  launch() {
    this.dialogRef.close({ json: this.json });
  }
}
