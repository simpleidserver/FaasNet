import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { StateMachineEvent } from "@stores/statemachines/models/statemachine-event.model";

@Component({
  selector: 'edit-evts-dialog-dialog',
  templateUrl: './editevent-dialog.component.html'
})
export class EditEventDialogComponent {
  editEventFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required])
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: StateMachineEvent,
    private dialogRef: MatDialogRef<EditEventDialogComponent>) {

    this._init(data);
  }

  isDisabled() {
    if (!this.editEventFormGroup.valid) {
      return true;
    }

    return false;
  }

  save() {
    if (this.isDisabled()) {
      return;
    }

    var record = new StateMachineEvent();
    record.name = this.editEventFormGroup.get('name')?.value;
    record.type = this.editEventFormGroup.get('type')?.value;
    record.kind = this.data.kind;
    record.source = this.data.source;
    this.dialogRef.close(record);
  }

  private _init(data: StateMachineEvent) {
    if (!data) {
      return;
    }

    this.editEventFormGroup.get('name')?.setValue(data.name);
    this.editEventFormGroup.get('type')?.setValue(data.type);
  }
}
