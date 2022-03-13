import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { OperationAction } from '@stores/statemachines/models/statemachine-operation-state.model';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { EditActionDialogComponent } from './editaction-dialog.component';

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
  supportedTypes: string[] = [''];
  functions: StateMachineFunction[] = [];
  actions: MatTableDataSource<OperationAction> = new MatTableDataSource<OperationAction>();

  constructor(
    private matDialog: MatDialog) {
    super();
  }

  override init(data: any): void {
    this.functions = (data as ActionsEditorData).functions;
    this.actions.data = (data as ActionsEditorData).actions;
  }

  removeAction(index: number) {
    this.actions.data.splice(index, 1);
    this.actions.data = this.actions.data;
  }

  addAction() {
    const dialogRef = this.matDialog.open(EditActionDialogComponent, {
      data: {
        functions: this.functions
      },
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }


      this.actions.data.push(e);
      this.actions.data = this.actions.data;
    });
  }

  editAction(action: OperationAction, index : number) {
    const dialogRef = this.matDialog.open(EditActionDialogComponent, {
      data: {
        functions: this.functions,
        action: action
      },
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      this.actions.data.splice(index, 1);
      this.actions.data.push(e);
      this.actions.data = this.actions.data;
    });
  }

  getType(action: OperationAction): string {
    if (action.functionRef) {
      const fn = this.functions.filter((f) => f.name == action.functionRef?.refName)[0];
      return fn.type +" : " + fn.operation;
    }

    return "";
  }

  save() {
    this.onClosed.emit(this.actions.data);
  }
}
