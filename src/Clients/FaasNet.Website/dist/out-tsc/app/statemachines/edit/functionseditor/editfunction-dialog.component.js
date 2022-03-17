import { __decorate, __param } from "tslib";
import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { OperationTypes } from "@stores/statemachines/models/operation-types.model";
import { StateMachineFunction } from "@stores/statemachines/models/statemachine-function.model";
let EditFunctionDialogComponent = class EditFunctionDialogComponent {
    constructor(data, dialogRef, openApiService, asyncApiService) {
        this.data = data;
        this.dialogRef = dialogRef;
        this.openApiService = openApiService;
        this.asyncApiService = asyncApiService;
        this.editFunctionFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            type: new FormControl('', [Validators.required])
        });
        this.editRestFormGroup = new FormGroup({
            url: new FormControl('', [Validators.required]),
            isOpenApiUrl: new FormControl(false),
            operationId: new FormControl('', [Validators.required])
        });
        this.editAsyncApiFormGroup = new FormGroup({
            url: new FormControl('', [Validators.required]),
            operationId: new FormControl('', [Validators.required])
        });
        this.editKubernetesFormGroup = new FormGroup({
            image: new FormControl('', [Validators.required]),
            version: new FormControl('', [Validators.required]),
            configuration: new FormControl()
        });
        this.openApiErrorMessage = null;
        this.asyncApiErrorMessage = null;
        this.operations = [];
        this.asyncApiOperations = [];
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
        this._init(data);
    }
    isDisabled() {
        var _a, _b, _c, _d;
        if (!this.editFunctionFormGroup.valid) {
            return true;
        }
        let type = (_a = this.editFunctionFormGroup.get('type')) === null || _a === void 0 ? void 0 : _a.value;
        if (type == 'kubernetes') {
            return !this.editKubernetesFormGroup.valid;
        }
        const isOpenApiUrl = (_b = this.editRestFormGroup.get('isOpenApiUrl')) === null || _b === void 0 ? void 0 : _b.value;
        if (type === OperationTypes.Rest && isOpenApiUrl) {
            return !this.editRestFormGroup.valid;
        }
        if (type === OperationTypes.Rest && !isOpenApiUrl) {
            return (_d = (_c = this.editRestFormGroup.get('url')) === null || _c === void 0 ? void 0 : _c.errors) === null || _d === void 0 ? void 0 : _d.required;
        }
        if (type === OperationTypes.AsyncApi && !this.editAsyncApiFormGroup.valid) {
            return true;
        }
        return false;
    }
    save() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
        if (this.isDisabled()) {
            return;
        }
        var record = new StateMachineFunction();
        record.name = (_a = this.editFunctionFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.value;
        let type = (_b = this.editFunctionFormGroup.get('type')) === null || _b === void 0 ? void 0 : _b.value;
        switch (type) {
            case 'kubernetes':
                type = OperationTypes.Custom;
                record.metadata = {
                    version: (_c = this.editKubernetesFormGroup.get('version')) === null || _c === void 0 ? void 0 : _c.value,
                    image: (_d = this.editKubernetesFormGroup.get('image')) === null || _d === void 0 ? void 0 : _d.value
                };
                var configuration = (_e = this.editKubernetesFormGroup.get('configuration')) === null || _e === void 0 ? void 0 : _e.value;
                if (configuration) {
                    try {
                        var o = JSON.parse(configuration);
                        record.metadata['configuration'] = o;
                    }
                    catch (_l) { }
                }
                break;
            case OperationTypes.Rest:
                var url = (_f = this.editRestFormGroup.get('url')) === null || _f === void 0 ? void 0 : _f.value;
                const isOpenApiUrl = (_g = this.editRestFormGroup.get('isOpenApiUrl')) === null || _g === void 0 ? void 0 : _g.value;
                if (isOpenApiUrl) {
                    url = url + '#' + ((_h = this.editRestFormGroup.get('operationId')) === null || _h === void 0 ? void 0 : _h.value);
                }
                record.operation = url;
                break;
            case OperationTypes.AsyncApi:
                var url = (_j = this.editAsyncApiFormGroup.get('url')) === null || _j === void 0 ? void 0 : _j.value;
                url = url + '#' + ((_k = this.editAsyncApiFormGroup.get('operationId')) === null || _k === void 0 ? void 0 : _k.value);
                record.operation = url;
                break;
        }
        record.type = type;
        this.dialogRef.close(record);
    }
    extractOpenApi(evt) {
        var _a;
        evt.preventDefault();
        (_a = this.editRestFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.setValue('');
        this._refreshOpenApiOperations();
    }
    extractAsyncApi(evt) {
        var _a;
        evt.preventDefault();
        (_a = this.editAsyncApiFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.setValue('');
        this._refreshAsyncApiOperations();
    }
    displaySelectedOpenApiOperation() {
        var _a;
        const operationId = (_a = this.editRestFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.value;
        const filtered = this.operations.filter((o) => o.operationId == operationId);
        if (filtered.length !== 1) {
            return "";
        }
        return "Summary: " + filtered[0].summary;
    }
    displaySelectedAsyncApiOperation() {
        var _a;
        const operationId = (_a = this.editAsyncApiFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.value;
        const filtered = this.asyncApiOperations.filter((o) => o.operationId == operationId);
        if (filtered.length !== 1) {
            return "";
        }
        return "Summary: " + filtered[0].summary;
    }
    _init(data) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
        if (!data) {
            return;
        }
        (_a = this.editFunctionFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue(data.name);
        (_b = this.editFunctionFormGroup.get('type')) === null || _b === void 0 ? void 0 : _b.setValue(data.type);
        if (data.metadata) {
            if (data.metadata.version && data.metadata.image) {
                (_c = this.editFunctionFormGroup.get('type')) === null || _c === void 0 ? void 0 : _c.setValue('kubernetes');
                (_d = this.editKubernetesFormGroup.get('version')) === null || _d === void 0 ? void 0 : _d.setValue(data.metadata.version);
                (_e = this.editKubernetesFormGroup.get('image')) === null || _e === void 0 ? void 0 : _e.setValue(data.metadata.image);
                if (data.metadata.configuration) {
                    (_f = this.editKubernetesFormGroup.get('configuration')) === null || _f === void 0 ? void 0 : _f.setValue(JSON.stringify(data.metadata.configuration));
                }
            }
        }
        if (data.type === OperationTypes.Rest) {
            const splittedOperation = (_g = data.operation) === null || _g === void 0 ? void 0 : _g.split('#');
            let isOpenApiUrl = false;
            let url = data.operation;
            if ((splittedOperation === null || splittedOperation === void 0 ? void 0 : splittedOperation.length) === 2) {
                url = splittedOperation[0];
                isOpenApiUrl = true;
                const operationId = splittedOperation[1];
                (_h = this.editRestFormGroup.get('operationId')) === null || _h === void 0 ? void 0 : _h.setValue(operationId);
            }
            (_j = this.editRestFormGroup.get('url')) === null || _j === void 0 ? void 0 : _j.setValue(url);
            (_k = this.editRestFormGroup.get('isOpenApiUrl')) === null || _k === void 0 ? void 0 : _k.setValue(isOpenApiUrl);
            if (isOpenApiUrl) {
                this._refreshOpenApiOperations();
            }
        }
        if (data.type === OperationTypes.AsyncApi) {
            const splittedOperation = (_l = data.operation) === null || _l === void 0 ? void 0 : _l.split('#');
            if (splittedOperation) {
                const url = splittedOperation[0];
                const operationId = splittedOperation[1];
                (_m = this.editAsyncApiFormGroup.get('operationId')) === null || _m === void 0 ? void 0 : _m.setValue(operationId);
                (_o = this.editAsyncApiFormGroup.get('url')) === null || _o === void 0 ? void 0 : _o.setValue(url);
                this._refreshAsyncApiOperations();
            }
        }
    }
    _refreshOpenApiOperations() {
        var _a;
        const url = (_a = this.editRestFormGroup.get('url')) === null || _a === void 0 ? void 0 : _a.value;
        this.operations = [{ 'operationId': 'Loading...' }];
        this.openApiService.getOperations(url).subscribe((e) => {
            var _a;
            this.openApiErrorMessage = null;
            this.operations = e;
            (_a = this.editRestFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.setValue(e[0].operationId);
        }, (e) => {
            this.operations = [];
            if (e.error && e.error.Message) {
                this.openApiErrorMessage = e.error.Message;
            }
            else {
                this.openApiErrorMessage = "An error occured while trying to get the operations from the OPENAPI URL";
            }
        });
    }
    _refreshAsyncApiOperations() {
        var _a;
        const url = (_a = this.editAsyncApiFormGroup.get('url')) === null || _a === void 0 ? void 0 : _a.value;
        this.asyncApiOperations = [{ 'operationId': 'Loading...' }];
        this.asyncApiService.getOperations(url).subscribe((e) => {
            var _a;
            this.asyncApiErrorMessage = null;
            this.asyncApiOperations = e;
            (_a = this.editAsyncApiFormGroup.get('operationId')) === null || _a === void 0 ? void 0 : _a.setValue(e[0].operationId);
        }, (e) => {
            if (e.error && e.error.Message) {
                this.asyncApiErrorMessage = e.error.Message;
            }
            else {
                this.asyncApiErrorMessage = "An error occured while trying to get the operations from the ASYNCAPI URL";
            }
        });
    }
};
EditFunctionDialogComponent = __decorate([
    Component({
        selector: 'edit-function-dialog-dialog',
        templateUrl: './editfunction-dialog.component.html'
    }),
    __param(0, Inject(MAT_DIALOG_DATA))
], EditFunctionDialogComponent);
export { EditFunctionDialogComponent };
//# sourceMappingURL=editfunction-dialog.component.js.map