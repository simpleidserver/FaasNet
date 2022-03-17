import { __decorate, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
let ExpressionEditorComponent = class ExpressionEditorComponent {
    constructor(data, dialogRef) {
        this.data = data;
        this.dialogRef = dialogRef;
        this._filter = "";
        this._json = "{}";
        this._result = "{}";
        this._modelFilterOptions = null;
        this.jsonEditorOptions = {
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
        this.filterOptions = { theme: 'vs', language: 'jq', isSingleLine: true };
        if (data && data.filter) {
            var filter = data.filter.replace('$', '')
                .replace('{', '')
                .replace('}', '')
                .trim();
            this._filter = filter;
        }
    }
    get filter() {
        return this._filter;
    }
    set filter(f) {
        this._filter = f;
        this.applyFilter();
    }
    get json() {
        return this._json;
    }
    set json(j) {
        this._json = j;
        this.updateFilter();
        this.applyFilter();
    }
    get result() {
        return this._result;
    }
    set result(r) {
        this._result = r;
    }
    save() {
        if (!this._filter || this.filter === "") {
            this.dialogRef.close({
                filter: null
            });
            return;
        }
        let filter = "${ " + this._filter + " }";
        this.dialogRef.close({
            filter: filter
        });
    }
    ngAfterViewInit() {
    }
    initFilterOptions(evt) {
        this._modelFilterOptions = evt.getModel();
    }
    updateFilter() {
        if (!this._modelFilterOptions) {
            return;
        }
        let json = null;
        try {
            json = JSON.parse(this._json);
        }
        catch (_a) { }
        Object.assign(this._modelFilterOptions, {
            json: json
        });
    }
    applyFilter() {
        let jObj = null;
        let result = "";
        try {
            jObj = JSON.parse(this._json);
        }
        catch (_a) { }
        if (jObj && this._filter) {
            try {
                const record = jq.json(jObj, this._filter);
                if (typeof record == "object") {
                    result = JSON.stringify(record);
                }
                else {
                    result = record.toString();
                }
            }
            catch (_b) { }
        }
        this.result = result;
    }
};
ExpressionEditorComponent = __decorate([
    Component({
        selector: 'expressioneditor',
        templateUrl: './expressioneditor.component.html',
        styleUrls: [
            './expressioneditor.component.scss',
            '../state-editor.component.scss'
        ]
    }),
    __param(0, Inject(MAT_DIALOG_DATA))
], ExpressionEditorComponent);
export { ExpressionEditorComponent };
//# sourceMappingURL=expressioneditor.component.js.map