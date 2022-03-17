import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
let ApplicationEditorComponent = class ApplicationEditorComponent {
    constructor() {
        this._application = undefined;
        this.updateApplicationFormGroup = new FormGroup({
            title: new FormControl(),
            description: new FormControl(),
            version: new FormControl()
        });
        this.closed = new EventEmitter();
    }
    get element() {
        return this._application;
    }
    set element(v) {
        this._application = v;
        this.init();
    }
    ngOnDestroy() {
        if (this._titleSubscription) {
            this._titleSubscription.unsubscribe();
        }
        if (this._descriptionSubscription) {
            this._descriptionSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    init() {
        var _a, _b, _c, _d, _e;
        const self = this;
        if (!self._application) {
            return;
        }
        self._titleSubscription = (_a = this.updateApplicationFormGroup.get('title')) === null || _a === void 0 ? void 0 : _a.valueChanges.subscribe((e) => {
            if (self._application) {
                self._application.title = e;
            }
        });
        self._descriptionSubscription = (_b = this.updateApplicationFormGroup.get('description')) === null || _b === void 0 ? void 0 : _b.valueChanges.subscribe((e) => {
            if (self._application) {
                self._application.description = e;
            }
        });
        (_c = this.updateApplicationFormGroup.get('title')) === null || _c === void 0 ? void 0 : _c.setValue(self._application.title);
        (_d = this.updateApplicationFormGroup.get('description')) === null || _d === void 0 ? void 0 : _d.setValue(self._application.description);
        (_e = this.updateApplicationFormGroup.get('version')) === null || _e === void 0 ? void 0 : _e.setValue(self._application.version);
    }
};
__decorate([
    Input()
], ApplicationEditorComponent.prototype, "element", null);
__decorate([
    Output()
], ApplicationEditorComponent.prototype, "closed", void 0);
ApplicationEditorComponent = __decorate([
    Component({
        selector: 'application-editor',
        templateUrl: './application-editor.component.html',
        styleUrls: [
            './application-editor.component.scss',
            '../editor.component.scss'
        ]
    })
], ApplicationEditorComponent);
export { ApplicationEditorComponent };
//# sourceMappingURL=application-editor.component.js.map