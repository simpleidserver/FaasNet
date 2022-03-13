import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { EditFunctionDialogComponent } from './editfunction-dialog.component';

export class FunctionsEditorData {
  functions: StateMachineFunction[] = [];
}

@Component({
  selector: 'edit-functions-statemachine',
  templateUrl: './functionseditor.component.html',
  styleUrls: [
    './functionseditor.component.scss',
  ]
})
export class FunctionsEditorComponent {
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  functions: MatTableDataSource<StateMachineFunction> = new MatTableDataSource<StateMachineFunction>();
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    this.functions.data = value.functions;
  }
  displayedColumns: string[] = ['actions', 'name', 'type', 'operation' ];

  constructor(private dialog: MatDialog) {
  }

  deleteFunction(i: number) {
    this.functions.data.splice(i, 1);
    this.functions.data = this.functions.data;
  }

  addFunction() {
    const dialogRef = this.dialog.open(EditFunctionDialogComponent, {
      data: null,
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((d) => {
      if (!d) {
        return;
      }

      this.functions.data.push(d);
      this.functions.data = this.functions.data;
    });
  }

  editFunction(fn: StateMachineFunction, index: number) {
    const dialogRef = this.dialog.open(EditFunctionDialogComponent, {
      data: fn,
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((d) => {
      if (!d) {
        return;
      }

      this.functions.data.splice(index, 1);
      this.functions.data = this.functions.data;
    });
  }
}
