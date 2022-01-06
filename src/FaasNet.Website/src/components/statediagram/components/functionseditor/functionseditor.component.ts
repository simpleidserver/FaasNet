import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';

export class FunctionsEditorData {
  functions: StateMachineFunction[] = [];
}

@Component({
  selector: 'functionseditor',
  templateUrl: './functionseditor.component.html',
  styleUrls: [
    './functionseditor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class FunctionsEditorComponent {
  functions: MatTableDataSource<StateMachineFunction> = new MatTableDataSource<StateMachineFunction>();
  editFunctionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required]),
    operation: new FormControl(),
  });
  editKubernetesFormGroup: FormGroup = new FormGroup({
    image: new FormControl('', [Validators.required]),
    version: new FormControl('', [Validators.required]),
    configuration: new FormControl()
  });
  displayedColumns: string[] = ['actions', 'name', 'type' ];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FunctionsEditorData,
    private dialogRef: MatDialogRef<FunctionsEditorComponent>) {
    this.functions.data = data.functions;
  }

  deleteFunction(i: number) {
    this.functions.data.splice(i, 1);
    this.functions.data = this.functions.data;
  }

  addFunction() {
    if (this.isDisabled()) {
      return;
    }

    let record = new StateMachineFunction();
    record.name = this.editFunctionFormGroup.get('name')?.value;
    record.operation = this.editFunctionFormGroup.get('operation')?.value;
    let type = this.editFunctionFormGroup.get('type')?.value;
    if (type === 'kubernetes') {
      type = 'custom';
      record.metadata = {
        version: this.editKubernetesFormGroup.get('version')?.value,
        image: this.editKubernetesFormGroup.get('image')?.value
      };
      var configuration = this.editKubernetesFormGroup.get('configuration')?.value;
      if (configuration) {
        try {
          var o = JSON.parse(configuration);
          record.metadata['configuration'] = o;
        } catch { }
      }
    }

    record.type = type;
    this.functions.data.push(record);
    this.functions.data = this.functions.data;
    this.editFunctionFormGroup.reset();
  }

  getOperationTranslationKey() {
    let type = this.editFunctionFormGroup.get('type')?.value;
    switch (type) {
      case 'rest':
        return 'API url or OPENAPI url';
    }

    return '';
  }

  isDisabled() {
    if (!this.editFunctionFormGroup.valid) {
      return true;
    }

    let type = this.editFunctionFormGroup.get('type')?.value;
    if (type == 'kubernetes') {
      return !this.editKubernetesFormGroup.valid;
    }

    return false;
  }

  save() {
    this.dialogRef.close(this.functions.data);
  }
}
