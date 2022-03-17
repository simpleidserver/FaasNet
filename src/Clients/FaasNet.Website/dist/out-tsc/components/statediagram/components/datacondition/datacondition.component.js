import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";
let DataConditionComponent = class DataConditionComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.conditionRefSubscription = null;
        this._dataCondition = null;
        this._transition = null;
        this.closed = new EventEmitter();
        this.updateDataConditionFormGroup = new FormGroup({
            condition: new FormControl()
        });
    }
    get transition() {
        return this._transition;
    }
    set transition(v) {
        this._transition = v;
        this._dataCondition = v;
        this.init();
    }
    ngOnInit() {
    }
    ngOnDestroy() {
        if (this.conditionRefSubscription) {
            this.conditionRefSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    editExpression() {
        let filter = "";
        const conditionFormControl = this.updateDataConditionFormGroup.get('condition');
        if (conditionFormControl) {
            filter = conditionFormControl.value;
        }
        const dialogRef = this.dialog.open(ExpressionEditorComponent, {
            width: '800px',
            data: {
                filter: filter
            }
        });
        dialogRef.afterClosed().subscribe((r) => {
            var _a, _b;
            if (!r) {
                return;
            }
            if (this._dataCondition) {
                this._dataCondition.condition = r.filter;
                (_a = this.updateDataConditionFormGroup.get('condition')) === null || _a === void 0 ? void 0 : _a.setValue((_b = this._dataCondition) === null || _b === void 0 ? void 0 : _b.condition);
            }
        });
    }
    init() {
        var _a, _b, _c;
        const self = this;
        this.ngOnDestroy();
        (_a = this.updateDataConditionFormGroup.get('condition')) === null || _a === void 0 ? void 0 : _a.setValue((_b = this._dataCondition) === null || _b === void 0 ? void 0 : _b.condition);
        this.conditionRefSubscription = (_c = this.updateDataConditionFormGroup.get('condition')) === null || _c === void 0 ? void 0 : _c.valueChanges.subscribe((e) => {
            if (self._dataCondition) {
                self._dataCondition.condition = e;
            }
        });
    }
};
__decorate([
    Output()
], DataConditionComponent.prototype, "closed", void 0);
__decorate([
    Input()
], DataConditionComponent.prototype, "transition", null);
DataConditionComponent = __decorate([
    Component({
        selector: 'datacondition-editor',
        templateUrl: './datacondition.component.html',
        styleUrls: ['../state-editor.component.scss']
    })
], DataConditionComponent);
export { DataConditionComponent };
//# sourceMappingURL=datacondition.component.js.map