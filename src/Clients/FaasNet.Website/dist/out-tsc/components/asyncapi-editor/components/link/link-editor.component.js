import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { LinkEventsEditorComponent, LinkEvtsEditorData } from "./evtseditor.component";
let LinkEditorComponent = class LinkEditorComponent {
    constructor(matPanelService) {
        this.matPanelService = matPanelService;
        this._link = undefined;
        this.updateLinkFormGroup = new FormGroup({});
        this.closed = new EventEmitter();
    }
    get link() {
        return this._link;
    }
    set link(v) {
        this._link = v;
    }
    ngOnDestroy() {
    }
    close() {
        this.closed.emit();
    }
    getMessages() {
        var _a;
        return (_a = this._link) === null || _a === void 0 ? void 0 : _a.evts.map(l => l.name).join(',');
    }
    editMessages() {
        const data = new LinkEvtsEditorData();
        if (this._link) {
            data.evts = this._link.evts;
        }
        this.matPanelService.open(LinkEventsEditorComponent, data);
    }
};
__decorate([
    Input()
], LinkEditorComponent.prototype, "link", null);
__decorate([
    Output()
], LinkEditorComponent.prototype, "closed", void 0);
LinkEditorComponent = __decorate([
    Component({
        selector: 'link-editor',
        templateUrl: './link-editor.component.html',
        styleUrls: [
            './link-editor.component.scss',
            '../editor.component.scss'
        ]
    })
], LinkEditorComponent);
export { LinkEditorComponent };
//# sourceMappingURL=link-editor.component.js.map