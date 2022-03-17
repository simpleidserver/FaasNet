import { __decorate } from "tslib";
import { Component, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { EditFunctionDialogComponent } from './editfunction-dialog.component';
export class FunctionsEditorData {
    constructor() {
        this.functions = [];
    }
}
let FunctionsEditorComponent = class FunctionsEditorComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this._stateMachineModel = new StateMachineModel();
        this.functions = new MatTableDataSource();
        this.displayedColumns = ['actions', 'name', 'type', 'operation'];
    }
    get stateMachine() {
        return this._stateMachineModel;
    }
    set stateMachine(value) {
        this._stateMachineModel = value;
        this.functions.data = value.functions;
    }
    deleteFunction(i) {
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
    editFunction(fn, index) {
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
};
__decorate([
    Input()
], FunctionsEditorComponent.prototype, "stateMachine", null);
FunctionsEditorComponent = __decorate([
    Component({
        selector: 'edit-functions-statemachine',
        templateUrl: './functionseditor.component.html',
        styleUrls: [
            './functionseditor.component.scss',
        ]
    })
], FunctionsEditorComponent);
export { FunctionsEditorComponent };
//# sourceMappingURL=functionseditor.component.js.map