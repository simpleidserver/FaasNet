import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { ActionDataFilter, OperationAction, OperationActionFunctionRef } from '@stores/statemachines/models/statemachine-operation-state.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { ExpressionEditorComponent } from '../expressioneditor/expressioneditor.component';

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
export class ActionsEditorComponent extends MatPanelContent {
  displayedColumns: string[] = ['actions', 'name', 'type'];
  functions: StateMachineFunction[] = [];
  actions: MatTableDataSource<OperationAction> = new MatTableDataSource<OperationAction>();
  jsonOptions: any = {
    theme: 'vs',
    language: 'json',
    minimap: { enabled: false },
    overviewRulerBorder: false,
    overviewRulerLanes: 0,
    lineNumbers: 'off',
    lineNumbersMinChars: 0,
    lineDecorationsWidth: 0,
    renderLineHighlight: 'none',
    scrollbar: {
      horizontal: 'hidden',
      vertical: 'hidden',
      alwaysConsumeMouseWheel: false,
    }
  };
  addActionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('1'),
    useResults: new FormControl('true'),
    results: new FormControl(),
    toStateData: new FormControl()
  });
  addFunctionFormGroup: FormGroup = new FormGroup({
    refName: new FormControl('', [Validators.required]),
    arguments: new FormControl()
  });

  constructor(private matDialog: MatDialog) {
    super();
  }

  override init(data: any): void {
    this.functions = (data as ActionsEditorData).functions;
    this.actions.data = (data as ActionsEditorData).actions;
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
        record.actionDataFilter = new ActionDataFilter();
        record.functionRef.refName = this.addFunctionFormGroup.get('refName')?.value;
        const useResults = Boolean(this.addActionFormGroup.get('useResults')?.value);
        record.actionDataFilter.useResults = useResults;
        if (useResults == true) {
          record.actionDataFilter.toStateData = this.addActionFormGroup.get('toStateData')?.value;
          record.actionDataFilter.results = this.addActionFormGroup.get('results')?.value;
        }

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
    this.onClosed.emit(this.actions.data);
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

  editResults() {
    const filter = this.addActionFormGroup.get('results')?.value;
    const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r || !r.filter) {
        return;
      }

      this.addActionFormGroup.get('results')?.setValue(r.filter);
    });
  }

  editToStateData() {
    const filter = this.addActionFormGroup.get('toStateData')?.value;
    const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r || !r.filter) {
        return;
      }

      this.addActionFormGroup.get('toStateData')?.setValue(r.filter);
    });
  }
}
