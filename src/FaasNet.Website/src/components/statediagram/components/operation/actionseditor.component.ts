import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { OperationAction, OperationActionFunctionRef } from '@stores/statemachines/models/statemachine-operation-state.model';

export class ActionsEditorData {
  functions: StateMachineFunction[] = [];
  actions: OperationAction[] = [];
}

@Component({
  selector: 'actionseditor',
  templateUrl: './actionseditor.component.html',
  styleUrls: [
    './actionseditor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class ActionsEditorComponent {
  displayedColumns: string[] = ['actions', 'name', 'type'];
  functions: StateMachineFunction[] = [];
  actions: MatTableDataSource<OperationAction> = new MatTableDataSource<OperationAction>();
  addActionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('1')
  });
  addFunctionFormGroup: FormGroup = new FormGroup({
    refName: new FormControl('', [Validators.required]),
    arguments: new FormControl()
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ActionsEditorData,
    private dialogRef: MatDialogRef<any>) {
    this.functions = [...data.functions];
    this.actions.data = [...data.actions];
  }

  removeAction(index : number) {
    this.actions.data.splice(index, 1);
    this.actions.data = this.actions.data;
  }

  addAction() {
    if (this.isDisabled()) {
      return;
    }

    let record = new OperationAction();
    record.name = this.addActionFormGroup.get('name')?.value;
    let type = this.addActionFormGroup.get('type')?.value;
    switch (type) {
      case '1':
        record.functionRef = new OperationActionFunctionRef();
        record.functionRef.refName = this.addFunctionFormGroup.get('refName')?.value;
        let args = this.addFunctionFormGroup.get('arguments')?.value;
        if (args) {
          try {
            var obj = JSON.parse(args);
            record.functionRef.arguments = obj;
          }
          catch { }
        }

        break;
    }

    this.actions.data.push(record);
    this.actions.data = this.actions.data;
    this.actions = this.actions;
    this.addFunctionFormGroup.reset();
    this.addActionFormGroup.reset();
  }

  getType(action: OperationAction) : string {
    if (action.functionRef) {
      return "function";
    }

    return "";
  }

  save() {
    this.dialogRef.close(this.actions.data);
  }

  isDisabled() {
    if (!this.addActionFormGroup.valid) {
      return true;
    }

    let type = this.addActionFormGroup.get('type')?.value;
    switch (type) {
      case '1':
        return !this.addFunctionFormGroup.valid;
    }

    return true;
  }
}
