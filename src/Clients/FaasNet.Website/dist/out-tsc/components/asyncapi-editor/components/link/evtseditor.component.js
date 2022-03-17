import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { EditLinkDialogComponent } from './editlink-dialog.component';
export class LinkEvtsEditorData {
    constructor() {
        this.evts = [];
    }
}
let LinkEventsEditorComponent = class LinkEventsEditorComponent extends MatPanelContent {
    constructor(matDialog) {
        super();
        this.matDialog = matDialog;
        this.displayedColumns = ['actions', 'name', 'title', 'description'];
        this.evts = new MatTableDataSource();
        this.functions = [];
    }
    init(data) {
        this.evts.data = data.evts;
    }
    addEvent() {
        const dialogRef = this.matDialog.open(EditLinkDialogComponent, {
            width: '600px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            this.evts.data.push(e);
            this.evts.data = this.evts.data;
        });
    }
    removeEvent(i) {
        this.evts.data.splice(i, 1);
        this.evts.data = this.evts.data;
    }
    editEvent(msg, i) {
        const dialogRef = this.matDialog.open(EditLinkDialogComponent, {
            data: msg,
            width: '600px'
        });
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            const existingMessage = this.evts.data[i];
            existingMessage.name = e.name;
            existingMessage.payload = e.payload;
            this.evts.data = this.evts.data;
        });
    }
    save() {
        this.onClosed.emit(this.evts.data);
    }
};
LinkEventsEditorComponent = __decorate([
    Component({
        selector: 'evtseditor',
        templateUrl: './evtseditor.component.html',
        styleUrls: [
            './evtseditor.component.scss',
            '../editor.component.scss'
        ]
    })
], LinkEventsEditorComponent);
export { LinkEventsEditorComponent };
//# sourceMappingURL=evtseditor.component.js.map