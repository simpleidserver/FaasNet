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
  displayedColumns: string[] = ['actions', 'name', 'type', 'operation'];

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
    let record = new StateMachineFunction();
    record.name = this.editFunctionFormGroup.get('name')?.value;
    record.operation = this.editFunctionFormGroup.get('operation')?.value;
    record.type = this.editFunctionFormGroup.get('type')?.value;
    this.functions.data.push(record);
    this.functions.data = this.functions.data;
    this.editFunctionFormGroup.reset();
  }

  getOperationTranslationKey() {
    let type = this.editFunctionFormGroup.get('type')?.value;
    switch (type) {
      case 'rest':
        return 'functions.openAPIOperation';
    }

    return 'functions.operation';
  }

  save() {
    this.dialogRef.close(this.functions.data);
  }
}
