import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { EditActionDialogComponent } from './editaction-dialog.component';
export class ActionsEditorData {
    constructor() {
        this.functions = [];
        this.actions = [];
    }
}
let ActionsEditorComponent = class ActionsEditorComponent extends MatPanelContent {
    constructor(matDialog) {
        super();
        this.matDialog = matDialog;
        this.displayedColumns = ['actions', 'name', 'type'];
        this.supportedTypes = [''];
        this.functions = [];
        this.actions = new MatTableDataSource();
    }
    init(data) {
        this.functions = data.functions;
        this.actions.data = data.actions;
    }
    removeAction(index) {
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
    editAction(action, index) {
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
    getType(action) {
        if (action.functionRef) {
            const fn = this.functions.filter((f) => { var _a; return f.name == ((_a = action.functionRef) === null || _a === void 0 ? void 0 : _a.refName); })[0];
            return fn.type + " : " + fn.operation;
        }
        return "";
    }
    save() {
        this.onClosed.emit(this.actions.data);
    }
};
ActionsEditorComponent = __decorate([
    Component({
        selector: 'actionseditor',
        templateUrl: './actionseditor.component.html',
        styleUrls: [
            './actionseditor.component.scss',
            '../state-editor.component.scss'
        ]
    })
], ActionsEditorComponent);
export { ActionsEditorComponent };
//# sourceMappingURL=actionseditor.component.js.map