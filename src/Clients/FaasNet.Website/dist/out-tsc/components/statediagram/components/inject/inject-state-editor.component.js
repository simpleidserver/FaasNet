import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";
let InjectStateEditorComponent = class InjectStateEditorComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this._state = null;
        this._injectState = null;
        this._data = "";
        this.nameSubscription = null;
        this.dataSubscription = null;
        this.closed = new EventEmitter();
        this.inputStateDataFilter = "";
        this.outputStateDataFilter = "";
        this.jsonOptions = {
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
        this.nameFormControl = new FormControl();
    }
    get state() {
        return this._state;
    }
    set state(v) {
        this._state = v;
        this._injectState = v;
        this.init();
    }
    get data() {
        return this._data;
    }
    set data(str) {
        this._data = str;
        if (this._injectState) {
            try {
                const data = JSON.parse(str);
                this._injectState.data = data;
            }
            catch (_a) { }
        }
    }
    get end() {
        var _a;
        return (_a = this._injectState) === null || _a === void 0 ? void 0 : _a.end;
    }
    ngOnInit() {
    }
    ngOnDestroy() {
        if (this.nameSubscription) {
            this.nameSubscription.unsubscribe();
        }
        if (this.dataSubscription) {
            this.dataSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    editInputStateDataFilter() {
        var _a, _b, _c;
        let filter = "";
        if ((_b = (_a = this._injectState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) === null || _b === void 0 ? void 0 : _b.input) {
            filter = (_c = this._injectState) === null || _c === void 0 ? void 0 : _c.stateDataFilter.input;
        }
        const dialogRef = this.dialog.open(ExpressionEditorComponent, {
            width: '800px',
            data: {
                filter: filter
            }
        });
        dialogRef.afterClosed().subscribe((r) => {
            var _a;
            if (!r) {
                return;
            }
            if ((_a = this._injectState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) {
                this._injectState.stateDataFilter.input = r.filter;
            }
            this.inputStateDataFilter = r.filter;
        });
    }
    editOutputStateDataFilter() {
        var _a, _b, _c;
        let filter = "";
        if ((_b = (_a = this._injectState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) === null || _b === void 0 ? void 0 : _b.output) {
            filter = (_c = this._injectState) === null || _c === void 0 ? void 0 : _c.stateDataFilter.output;
        }
        const dialogRef = this.dialog.open(ExpressionEditorComponent, {
            width: '800px',
            data: {
                filter: filter
            }
        });
        dialogRef.afterClosed().subscribe((r) => {
            var _a;
            if (!r) {
                return;
            }
            if ((_a = this._injectState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) {
                this._injectState.stateDataFilter.output = r.filter;
            }
            this.outputStateDataFilter = r.filter;
        });
    }
    init() {
        var _a;
        const self = this;
        this.ngOnDestroy();
        let json = "{}";
        if (this._injectState && this._injectState.data) {
            json = JSON.stringify(this._injectState.data);
        }
        this._data = json;
        this.nameFormControl.setValue((_a = this._injectState) === null || _a === void 0 ? void 0 : _a.name);
        this.nameSubscription = this.nameFormControl.valueChanges.subscribe((e) => {
            if (self._injectState) {
                self._injectState.name = e;
            }
        });
    }
};
__decorate([
    Output()
], InjectStateEditorComponent.prototype, "closed", void 0);
__decorate([
    Input()
], InjectStateEditorComponent.prototype, "state", null);
InjectStateEditorComponent = __decorate([
    Component({
        selector: 'inject-state-editor',
        templateUrl: './inject-state-editor.component.html',
        styleUrls: ['../state-editor.component.scss']
    })
], InjectStateEditorComponent);
export { InjectStateEditorComponent };
//# sourceMappingURL=inject-state-editor.component.js.map