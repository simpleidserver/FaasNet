import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ActionsEditorComponent, ActionsEditorData } from "./actionseditor.component";
let OperationStateEditorComponent = class OperationStateEditorComponent {
    constructor(matPanelService) {
        this.matPanelService = matPanelService;
        this.nameSubscription = null;
        this.actionModeSubscription = null;
        this._state = null;
        this._operationState = null;
        this.stateMachine = null;
        this.closed = new EventEmitter();
        this.updateOperationFormGroup = new FormGroup({
            name: new FormControl(),
            end: new FormControl(),
            actionMode: new FormControl()
        });
    }
    get state() {
        return this._state;
    }
    set state(v) {
        this._state = v;
        this._operationState = v;
        this.init();
    }
    get end() {
        var _a;
        return (_a = this._operationState) === null || _a === void 0 ? void 0 : _a.end;
    }
    ngOnInit() {
    }
    ngOnDestroy() {
        if (this.nameSubscription) {
            this.nameSubscription.unsubscribe();
        }
        if (this.actionModeSubscription) {
            this.actionModeSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    getActions() {
        var _a;
        return (_a = this._operationState) === null || _a === void 0 ? void 0 : _a.actions.map((a) => a.name).join(',');
    }
    editActions() {
        var data = new ActionsEditorData();
        if (this.stateMachine) {
            data.functions = this.stateMachine.functions;
        }
        if (this._operationState) {
            data.actions = this._operationState.actions;
        }
        this.matPanelService.open(ActionsEditorComponent, data);
    }
    init() {
        var _a, _b, _c, _d, _e, _f, _g;
        const self = this;
        if (self._operationState) {
            (_a = self.updateOperationFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue(self._operationState.name);
            (_b = self.updateOperationFormGroup.get('actionMode')) === null || _b === void 0 ? void 0 : _b.setValue(self._operationState.actionMode);
        }
        (_c = self.updateOperationFormGroup.get('end')) === null || _c === void 0 ? void 0 : _c.setValue((_d = self._operationState) === null || _d === void 0 ? void 0 : _d.end);
        (_e = self.updateOperationFormGroup.get('end')) === null || _e === void 0 ? void 0 : _e.disable();
        this.nameSubscription = (_f = this.updateOperationFormGroup.get('name')) === null || _f === void 0 ? void 0 : _f.valueChanges.subscribe((e) => {
            if (self.updateOperationFormGroup && self._operationState) {
                self._operationState.name = e;
            }
        });
        this.actionModeSubscription = (_g = this.updateOperationFormGroup.get('actionMode')) === null || _g === void 0 ? void 0 : _g.valueChanges.subscribe((e) => {
            if (self.updateOperationFormGroup && self._operationState) {
                self._operationState.actionMode = e;
            }
        });
    }
};
__decorate([
    Input()
], OperationStateEditorComponent.prototype, "stateMachine", void 0);
__decorate([
    Input()
], OperationStateEditorComponent.prototype, "state", null);
__decorate([
    Output()
], OperationStateEditorComponent.prototype, "closed", void 0);
OperationStateEditorComponent = __decorate([
    Component({
        selector: 'operation-state-editor',
        templateUrl: './operation-state-editor.component.html',
        styleUrls: [
            './operation-state-editor.component.scss',
            '../state-editor.component.scss'
        ]
    })
], OperationStateEditorComponent);
export { OperationStateEditorComponent };
//# sourceMappingURL=operation-state-editor.component.js.map