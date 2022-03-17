import { __decorate, __param } from "tslib";
import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { OperationTypes } from "@stores/statemachines/models/operation-types.model";
import { ActionDataFilter, OperationAction, OperationActionFunctionRef } from "@stores/statemachines/models/statemachine-operation-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";
class JSchemaStandardTypes {
}
JSchemaStandardTypes.Integer = "integer";
JSchemaStandardTypes.String = "string";
JSchemaStandardTypes.Array = "array";
JSchemaStandardTypes.Object = "object";
JSchemaStandardTypes.All = [JSchemaStandardTypes.Integer, JSchemaStandardTypes.String, JSchemaStandardTypes.Array, JSchemaStandardTypes.Object];
export class EditActionDialogData {
    constructor() {
        this.functions = [];
        this.action = null;
    }
}
let EditActionDialogComponent = class EditActionDialogComponent {
    constructor(data, dialogRef, openApiService, asyncApiService, matDialog) {
        this.data = data;
        this.dialogRef = dialogRef;
        this.openApiService = openApiService;
        this.asyncApiService = asyncApiService;
        this.matDialog = matDialog;
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
        this.functions = [];
        this.addActionFormGroup = new FormGroup({
            name: new FormControl('', [Validators.required]),
            type: new FormControl(''),
            useResults: new FormControl('true'),
            results: new FormControl(),
            toStateData: new FormControl()
        });
        this.addFunctionFormGroup = new FormGroup({
            refName: new FormControl('', [Validators.required]),
            queries: new FormControl(''),
            properties: new FormControl('')
        });
        this._init(data);
    }
    save() {
        var _a, _b, _c, _d, _e, _f, _g, _h;
        if (this.isDisabled()) {
            return;
        }
        let record = new OperationAction();
        record.name = (_a = this.addActionFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.value;
        let type = (_b = this.addActionFormGroup.get('type')) === null || _b === void 0 ? void 0 : _b.value;
        switch (type) {
            case '1':
                record.functionRef = new OperationActionFunctionRef();
                record.actionDataFilter = new ActionDataFilter();
                record.functionRef.refName = (_c = this.addFunctionFormGroup.get('refName')) === null || _c === void 0 ? void 0 : _c.value;
                const fn = this.functions.filter((f) => { var _a; return f.name === ((_a = record.functionRef) === null || _a === void 0 ? void 0 : _a.refName); })[0];
                const useResults = Boolean((_d = this.addActionFormGroup.get('useResults')) === null || _d === void 0 ? void 0 : _d.value);
                record.actionDataFilter.useResults = useResults;
                if (useResults == true) {
                    const toStateData = (_e = this.addActionFormGroup.get('toStateData')) === null || _e === void 0 ? void 0 : _e.value;
                    const results = (_f = this.addActionFormGroup.get('results')) === null || _f === void 0 ? void 0 : _f.value;
                    if (toStateData) {
                        record.actionDataFilter.toStateData = toStateData;
                    }
                    if (results) {
                        record.actionDataFilter.results = results;
                    }
                }
                let args = null;
                var properties = {};
                var propertiesStr = (_g = this.addFunctionFormGroup.get('properties')) === null || _g === void 0 ? void 0 : _g.value;
                if (propertiesStr) {
                    properties = JSON.parse(propertiesStr);
                }
                switch (fn.type) {
                    case OperationTypes.AsyncApi:
                        args = properties;
                        break;
                    case OperationTypes.Rest:
                        var queries = {};
                        var queriesStr = (_h = this.addFunctionFormGroup.get('queries')) === null || _h === void 0 ? void 0 : _h.value;
                        if (queriesStr) {
                            queries = JSON.parse(queriesStr);
                        }
                        if (Object.keys(queries).length > 0) {
                            args['queries'] = queries;
                        }
                        if (Object.keys(properties).length > 0) {
                            args['properties'] = properties;
                        }
                        break;
                }
                if (args !== null) {
                    record.functionRef.arguments = args;
                }
                break;
        }
        this.dialogRef.close(record);
    }
    isDisabled() {
        var _a;
        if (!this.addActionFormGroup.valid) {
            return true;
        }
        let type = (_a = this.addActionFormGroup.get('type')) === null || _a === void 0 ? void 0 : _a.value;
        switch (type) {
            case '1':
                return !this.addFunctionFormGroup.valid;
        }
        return true;
    }
    isOpenApiUrl() {
        var _a, _b;
        const refName = (_a = this.addFunctionFormGroup.get('refName')) === null || _a === void 0 ? void 0 : _a.value;
        if (!refName) {
            return false;
        }
        const fn = this.functions.filter((f) => f.name === refName && f.type === OperationTypes.Rest)[0];
        if (!fn) {
            return false;
        }
        return ((_b = fn.operation) === null || _b === void 0 ? void 0 : _b.split('#').length) === 2;
    }
    isAsyncApiUrl() {
        var _a, _b;
        const refName = (_a = this.addFunctionFormGroup.get('refName')) === null || _a === void 0 ? void 0 : _a.value;
        if (!refName) {
            return false;
        }
        const fn = this.functions.filter((f) => f.name === refName && f.type === OperationTypes.AsyncApi)[0];
        if (!fn) {
            return false;
        }
        return ((_b = fn.operation) === null || _b === void 0 ? void 0 : _b.split('#').length) === 2;
    }
    editResults() {
        var _a;
        const filter = (_a = this.addActionFormGroup.get('results')) === null || _a === void 0 ? void 0 : _a.value;
        const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
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
            (_a = this.addActionFormGroup.get('results')) === null || _a === void 0 ? void 0 : _a.setValue(r.filter);
        });
    }
    editToStateData() {
        var _a;
        const filter = (_a = this.addActionFormGroup.get('toStateData')) === null || _a === void 0 ? void 0 : _a.value;
        const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
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
            (_a = this.addActionFormGroup.get('toStateData')) === null || _a === void 0 ? void 0 : _a.setValue(r.filter);
        });
    }
    generateFakeArguments(evt) {
        var _a;
        evt.preventDefault();
        const refName = (_a = this.addFunctionFormGroup.get('refName')) === null || _a === void 0 ? void 0 : _a.value;
        const fn = this.functions.filter((f) => f.name === refName)[0];
        switch (fn.type) {
            case OperationTypes.AsyncApi:
                this._generateFakeAsyncApiArguments(fn);
                break;
            case OperationTypes.Rest:
                this._generateFakeOpenApiArguments(fn);
                break;
        }
    }
    _init(data) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
        if (data.functions) {
            this.functions = data.functions;
        }
        if (!data.action) {
            return;
        }
        (_a = this.addActionFormGroup.get('name')) === null || _a === void 0 ? void 0 : _a.setValue((_b = data.action) === null || _b === void 0 ? void 0 : _b.name);
        if (data.action.functionRef) {
            (_c = this.addActionFormGroup.get('type')) === null || _c === void 0 ? void 0 : _c.setValue('1');
            const refName = data.action.functionRef.refName;
            (_d = this.addFunctionFormGroup.get('refName')) === null || _d === void 0 ? void 0 : _d.setValue(refName);
            const refFunction = this.functions.filter((f) => f.name == refName)[0];
            switch (refFunction.type) {
                case OperationTypes.AsyncApi:
                    (_e = this.addFunctionFormGroup.get('properties')) === null || _e === void 0 ? void 0 : _e.setValue(JSON.stringify(data.action.functionRef.arguments, null, '\t'));
                    break;
                case OperationTypes.Rest:
                    let properties = data.action.functionRef.arguments['properties'];
                    let queries = data.action.functionRef.arguments['queries'];
                    properties = properties !== null && properties !== void 0 ? properties : {};
                    queries = queries !== null && queries !== void 0 ? queries : {};
                    (_f = this.addFunctionFormGroup.get('properties')) === null || _f === void 0 ? void 0 : _f.setValue(JSON.stringify(properties, null, '\t'));
                    (_g = this.addFunctionFormGroup.get('queries')) === null || _g === void 0 ? void 0 : _g.setValue(JSON.stringify(queries, null, '\t'));
                    break;
            }
        }
        if (data.action.actionDataFilter) {
            (_h = this.addActionFormGroup.get('useResults')) === null || _h === void 0 ? void 0 : _h.setValue(data.action.actionDataFilter.useResults.toString());
            (_j = this.addActionFormGroup.get('toStateData')) === null || _j === void 0 ? void 0 : _j.setValue(data.action.actionDataFilter.toStateData);
            (_k = this.addActionFormGroup.get('results')) === null || _k === void 0 ? void 0 : _k.setValue(data.action.actionDataFilter.results);
        }
    }
    _generateFakeAsyncApiArguments(fn) {
        var _a;
        const splitted = (_a = fn.operation) === null || _a === void 0 ? void 0 : _a.split('#');
        if (splitted) {
            this.asyncApiService.getOperation(splitted[0], splitted[1]).subscribe((e) => {
                var _a;
                const payload = e.asyncApiOperation.message.payload;
                const components = e.components;
                let json = {};
                this._extractJsonFromComponent(payload['$ref'], json, components);
                (_a = this.addFunctionFormGroup.get('properties')) === null || _a === void 0 ? void 0 : _a.setValue(JSON.stringify(json, undefined, 4));
            }, () => {
            });
        }
    }
    _generateFakeOpenApiArguments(fn) {
        var _a;
        const applicationJson = "application/json";
        const splitted = (_a = fn.operation) === null || _a === void 0 ? void 0 : _a.split('#');
        if (splitted) {
            this.openApiService.getOperation(splitted[0], splitted[1]).subscribe((e) => {
                var _a, _b;
                var queryParameters = {};
                var bodyParameters = {};
                const openApiOperation = e.openApiOperation;
                const components = e.components;
                if (openApiOperation.parameters) {
                    openApiOperation.parameters.forEach((p) => {
                        queryParameters = this._extractPathParameter(p, components);
                    });
                }
                if (openApiOperation.requestBody && openApiOperation.requestBody.content && openApiOperation.requestBody.content[applicationJson]) {
                    const prop = openApiOperation.requestBody.content[applicationJson];
                    bodyParameters = this._extractPathParameter(prop, components);
                }
                (_a = this.addFunctionFormGroup.get('queries')) === null || _a === void 0 ? void 0 : _a.setValue(JSON.stringify(queryParameters, undefined, 4));
                (_b = this.addFunctionFormGroup.get('properties')) === null || _b === void 0 ? void 0 : _b.setValue(JSON.stringify(bodyParameters, undefined, 4));
            }, () => {
            });
        }
    }
    _extractPathParameter(parameter, components, json = null, isArray = false) {
        var type = this._getJSchemaStandardType(parameter.schema["type"]);
        if (!json) {
            json = {};
            if (type == JSchemaStandardTypes.Array) {
                json = [];
                isArray = true;
            }
        }
        var name = parameter.name;
        switch (type) {
            case JSchemaStandardTypes.String:
                var value = this._getDefaultSchemaStrValue(parameter.schema);
                if (!isArray) {
                    json[name] = value;
                }
                else {
                    json[name].push(value);
                }
                break;
            case JSchemaStandardTypes.Integer:
                if (!isArray) {
                    json[name] = 0;
                }
                else {
                    json[name].push(0);
                }
                json[name] = 0;
                break;
            case JSchemaStandardTypes.Array:
                var arr = [];
                this._extractPathParameter(parameter.items, arr, components, true);
                if (!isArray) {
                    json[name] = arr;
                }
                else {
                    arr.forEach((r) => json.push(r));
                }
                break;
            default:
                const ref = parameter.schema["$ref"];
                this._extractJsonFromComponent(ref, json, components);
                break;
        }
        return json;
    }
    _extractJsonFromComponent(reference, json, components) {
        const component = this._getComponent(reference, components);
        for (var key in component.properties) {
            const property = component.properties[key];
            this._extractJsonFromProperty(property, key, json, components, false);
        }
    }
    _extractJsonFromProperty(property, propertyName, json, components, isArray = false) {
        var type = this._getJSchemaStandardType(property.type);
        switch (type) {
            case JSchemaStandardTypes.Integer:
                if (!isArray) {
                    json[propertyName] = 0;
                }
                else {
                    json.push(0);
                }
                break;
            case JSchemaStandardTypes.String:
                let str = this._getDefaultSchemaStrValue(property);
                if (!isArray) {
                    json[propertyName] = str;
                }
                else {
                    json.push(str);
                }
                break;
            case JSchemaStandardTypes.Array:
                var arr = [];
                this._extractJsonFromProperty(property.items, "", arr, components, true);
                if (!isArray) {
                    json[propertyName] = arr;
                }
                else {
                    arr.forEach((r) => json.push(r));
                }
                break;
            case JSchemaStandardTypes.Object:
                var obj = {};
                for (var key in property.properties) {
                    const child = property.properties[key];
                    this._extractJsonFromProperty(child, key, obj, components);
                }
                break;
            default:
                var obj = {};
                this._extractJsonFromComponent(property["$ref"], obj, components);
                if (!isArray) {
                    json[propertyName] = obj;
                }
                else {
                    json.push(obj);
                }
                break;
        }
    }
    _getJSchemaStandardType(type) {
        if (Array.isArray(type)) {
            const filtered = type.filter((t) => JSchemaStandardTypes.All.filter(a => a === t).length > 0);
            return filtered[0];
        }
        return type;
    }
    _getDefaultSchemaStrValue(schema) {
        if (schema.default) {
            return schema.default;
        }
        return "str";
    }
    _getComponent(ref, components) {
        const splittedRef = ref.split('/');
        const name = splittedRef[splittedRef.length - 1];
        return components.schemas[name];
    }
};
EditActionDialogComponent = __decorate([
    Component({
        selector: 'edit-action-dialog-dialog',
        templateUrl: './editaction-dialog.component.html'
    }),
    __param(0, Inject(MAT_DIALOG_DATA))
], EditActionDialogComponent);
export { EditActionDialogComponent };
//# sourceMappingURL=editaction-dialog.component.js.map