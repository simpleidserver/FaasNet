import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';

@Component({
  selector: 'add-messagedef',
  templateUrl: './add-message.component.html'
})
export class AddMessageDefComponent {
  addMessageDefFormGroup: FormGroup = new FormGroup({
    name  : new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    jsonSchema: new FormControl('', [Validators.required])
  });
  jsonOptions: any = { theme: 'vs', language: 'json' };
  isEditable: boolean = false;

  constructor(
    private dialogRef: MatDialogRef<AddMessageDefComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MessageDefinitionResult) {
    if (data) {
      this.addMessageDefFormGroup.get('name')?.setValue(data.name);
      this.addMessageDefFormGroup.get('description')?.setValue(data.description);
      this.addMessageDefFormGroup.get('jsonSchema')?.setValue(data.jsonSchema);
      this.isEditable = true;
    }
  }

  save() {
    if (!this.addMessageDefFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addMessageDefFormGroup.value);
  }
}
