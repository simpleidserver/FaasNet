import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { SwitchStateMachineState } from "@stores/statemachines/models/statemachine-switch-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";
let SwitchStateEditorComponent = class SwitchStateEditorComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.nameSubscription = null;
        this.typeSubscription = null;
        this._state = null;
        this._switchState = null;
        this.closed = new EventEmitter();
        this.updateSwitchFormGroup = new FormGroup({
            name: new FormControl(),
            type: new FormControl(),
            inputStateDataFilter: new FormControl(),
            outputStateDataFilter: new FormControl(),
            end: new FormControl()
        });
    }
    get state() {
        return this._state;
    }
    set state(v) {
        this._state = v;
        this._switchState = v;
        this.init();
    }
    get end() {
        var _a;
        return (_a = this._switchState) === null || _a === void 0 ? void 0 : _a.end;
    }
    ngOnInit() {
    }
    ngOnDestroy() {
        if (this.nameSubscription) {
            this.nameSubscription.unsubscribe();
        }
        if (this.typeSubscription) {
            this.typeSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    editInputStateDataFilter() {
        var _a, _b, _c;
        let filter = "";
        if ((_b = (_a = this._switchState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) === null || _b === void 0 ? void 0 : _b.input) {
            filter = (_c = this._switchState) === null || _c === void 0 ? void 0 : _c.stateDataFilter.input;
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
            if ((_a = this._switchState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) {
                this._switchState.stateDataFilter.input = r.filter;
            }
            (_b = this.updateSwitchFormGroup.get('inputStateDataFilter')) === null || _b === void 0 ? void 0 : _b.setValue(r.filter);
        });
    }
    editOutputStateDataFilter() {
        var _a, _b, _c;
        let filter = "";
        if ((_b = (_a = this._switchState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) === null || _b === void 0 ? void 0 : _b.output) {
            filter = (_c = this._switchState) === null || _c === void 0 ? void 0 : _c.stateDataFilter.output;
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
            if ((_a = this._switchState) === null || _a === void 0 ? void 0 : _a.stateDataFilter) {
                this._switchState.stateDataFilter.output = r.filter;
            }
            (_b = this.updateSwitchFormGroup.get('outputStateDataFilter')) === null || _b === void 0 ? void 0 : _b.setValue(r.filter);
        });
    }
    init() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j;
        const self = this;
        if (self._switchState) {
            (_a = self.updateSwitchFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue(self._switchState.name);
            if (((_b = this._switchState) === null || _b === void 0 ? void 0 : _b.dataConditions) && this._switchState.dataConditions.length > 0) {
                (_c = self.updateSwitchFormGroup.get('type')) === null || _c === void 0 ? void 0 : _c.setValue('data');
            }
            else {
                (_d = self.updateSwitchFormGroup.get('type')) === null || _d === void 0 ? void 0 : _d.setValue(self._switchState.switchType);
            }
        }
        (_e = self.updateSwitchFormGroup.get('end')) === null || _e === void 0 ? void 0 : _e.setValue((_f = self._switchState) === null || _f === void 0 ? void 0 : _f.end);
        (_g = self.updateSwitchFormGroup.get('end')) === null || _g === void 0 ? void 0 : _g.disable();
        this.nameSubscription = (_h = this.updateSwitchFormGroup.get('name')) === null || _h === void 0 ? void 0 : _h.valueChanges.subscribe((e) => {
            if (self.updateSwitchFormGroup && self._switchState) {
                self._switchState.name = e;
            }
        });
        this.typeSubscription = (_j = this.updateSwitchFormGroup.get('type')) === null || _j === void 0 ? void 0 : _j.valueChanges.subscribe((e) => {
            var _a, _b, _c;
            switch (e) {
                case SwitchStateMachineState.DATA_TYPE:
                    (_a = self._switchState) === null || _a === void 0 ? void 0 : _a.switchToDataCondition();
                    break;
                case SwitchStateMachineState.EVENT_TYPE:
                    (_b = self._switchState) === null || _b === void 0 ? void 0 : _b.switchToEventCondition();
                    break;
            }
            if (self._switchState) {
                (_c = this._switchState) === null || _c === void 0 ? void 0 : _c.updated.emit(self._switchState);
            }
        });
    }
};
__decorate([
    Input()
], SwitchStateEditorComponent.prototype, "state", null);
__decorate([
    Output()
], SwitchStateEditorComponent.prototype, "closed", void 0);
SwitchStateEditorComponent = __decorate([
    Component({
        selector: 'switch-state-editor',
        templateUrl: './switch-state-editor.component.html',
        styleUrls: ['../state-editor.component.scss']
    })
], SwitchStateEditorComponent);
export { SwitchStateEditorComponent };
//# sourceMappingURL=switch-state-editor.component.js.map