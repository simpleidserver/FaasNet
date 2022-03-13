import { Component, Inject } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, ValidationErrors, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Message } from "../../models/message";

function jsonValidator(control: AbstractControl): ValidationErrors | null {
  try {
    if (!control.value) {
      return null;
    }

    JSON.parse(control.value);
  } catch (e) {
    return { jsonInvalid: true };
  }

  return null;
};

@Component({
  selector: 'edit-link-dialog-dialog',
  templateUrl: './editlink-dialog.component.html'
})
export class EditLinkDialogComponent {
  jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
  editLinkFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', Validators.required),
    payload: new FormControl('', [ Validators.required, jsonValidator ])
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: Message,
    private dialogRef: MatDialogRef<EditLinkDialogComponent>) {
    this._init(data);
  }

  save() {
    if (!this.editLinkFormGroup.valid) {
      return;
    }

    const val = this.editLinkFormGroup.value;
    this.dialogRef.close({
      name: val.name,
      payload : JSON.parse(val.payload)
    });
  }

  private _init(data: Message) {
    if (!data) {
      return;
    }

    this.editLinkFormGroup.get('name')?.setValue(data.name);
    this.editLinkFormGroup.get('payload')?.setValue(JSON.stringify(data.payload, null, "\t"));
  }
}
