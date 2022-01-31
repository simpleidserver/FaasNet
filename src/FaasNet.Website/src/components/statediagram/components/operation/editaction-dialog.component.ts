import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { AsyncApiService } from "@stores/asyncapi/services/asyncapi.service";
import { OpenApiService } from "@stores/openapi/services/openapi.service";
import { OperationTypes } from "@stores/statemachines/models/operation-types.model";
import { StateMachineFunction } from "@stores/statemachines/models/statemachine-function.model";
import { ActionDataFilter, OperationAction, OperationActionFunctionRef } from "@stores/statemachines/models/statemachine-operation-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";

class JSchemaStandardTypes {
  public static Integer: string = "integer";
  public static String: string = "string";
  public static Array: string = "array";
  public static Object: string = "object";
  public static All: string[] = [JSchemaStandardTypes.Integer, JSchemaStandardTypes.String, JSchemaStandardTypes.Array, JSchemaStandardTypes.Object]
}

export class EditActionDialogData {
  functions: StateMachineFunction[] = [];
  action: OperationAction | null = null;
}

@Component({
  selector: 'edit-action-dialog-dialog',
  templateUrl: './editaction-dialog.component.html'
})
export class EditActionDialogComponent {
  jsonOptions: any = {
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
  functions: StateMachineFunction[] = [];
  addActionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl(''),
    useResults: new FormControl('true'),
    results: new FormControl(),
    toStateData: new FormControl()
  });
  addFunctionFormGroup: FormGroup = new FormGroup({
    refName: new FormControl('', [Validators.required]),
    queries: new FormControl(''),
    properties: new FormControl('')
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EditActionDialogData,
    private dialogRef: MatDialogRef<EditActionDialogComponent>,
    private openApiService: OpenApiService,
    private asyncApiService: AsyncApiService,
    private matDialog: MatDialog) {
    this._init(data);
  }

  save() {
    if (this.isDisabled()) {
      return;
    }

    let record = new OperationAction();
    record.name = this.addActionFormGroup.get('name')?.value;
    let type = this.addActionFormGroup.get('type')?.value;
    switch (type) {
      case '1':
        record.functionRef = new OperationActionFunctionRef();
        record.actionDataFilter = new ActionDataFilter();
        record.functionRef.refName = this.addFunctionFormGroup.get('refName')?.value;
        const fn = this.functions.filter((f) => f.name === record.functionRef?.refName)[0];
        const useResults = Boolean(this.addActionFormGroup.get('useResults')?.value);
        record.actionDataFilter.useResults = useResults;
        if (useResults == true) {
          const toStateData = this.addActionFormGroup.get('toStateData')?.value;
          const results = this.addActionFormGroup.get('results')?.value;
          if (toStateData) {
            record.actionDataFilter.toStateData = toStateData;
          }

          if (results) {
            record.actionDataFilter.results = results;
          }
        }

        let args: any = {};
        var properties = JSON.parse(this.addFunctionFormGroup.get('properties')?.value);
        switch (fn.type) {
          case OperationTypes.AsyncApi:
            args = properties;
            break;
          case OperationTypes.Rest:
            const queries = JSON.parse(this.addFunctionFormGroup.get('queries')?.value);
            if (Object.keys(queries).length > 0) {
              args['queries'] = queries;
            }

            if (Object.keys(properties).length > 0) {
              args['properties'] = properties;
            }
            break;
        }

        record.functionRef.arguments = args;
        break;
    }

    this.dialogRef.close(record);
  }

  isDisabled() {
    if (!this.addActionFormGroup.valid) {
      return true;
    }

    let type = this.addActionFormGroup.get('type')?.value;
    switch (type) {
      case '1':
        return !this.addFunctionFormGroup.valid;
    }

    return true;
  }

  isOpenApiUrl() {
    const refName = this.addFunctionFormGroup.get('refName')?.value;
    if (!refName) {
      return false;
    }

    const fn = this.functions.filter((f) => f.name === refName && f.type === OperationTypes.Rest)[0];
    if (!fn) {
      return false;
    }

    return fn.operation?.split('#').length === 2;
  }

  isAsyncApiUrl() {
    const refName = this.addFunctionFormGroup.get('refName')?.value;
    if (!refName) {
      return false;
    }

    const fn = this.functions.filter((f) => f.name === refName && f.type === OperationTypes.AsyncApi)[0];
    if (!fn) {
      return false;
    }

    return fn.operation?.split('#').length === 2;
  }

  editResults() {
    const filter = this.addActionFormGroup.get('results')?.value;
    const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      this.addActionFormGroup.get('results')?.setValue(r.filter);
    });
  }

  editToStateData() {
    const filter = this.addActionFormGroup.get('toStateData')?.value;
    const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      this.addActionFormGroup.get('toStateData')?.setValue(r.filter);
    });
  }

  generateFakeArguments(evt: any) {
    evt.preventDefault();
    const refName = this.addFunctionFormGroup.get('refName')?.value;
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

  private _init(data : EditActionDialogData) {
    if (data.functions) {
      this.functions = data.functions;
    }

    if (!data.action) {
      return;
    }

    this.addActionFormGroup.get('name')?.setValue(data.action?.name);
    if (data.action.functionRef) {
      this.addActionFormGroup.get('type')?.setValue('1');
      const refName = data.action.functionRef.refName;
      this.addFunctionFormGroup.get('refName')?.setValue(refName);
      const refFunction = this.functions.filter((f) => f.name == refName)[0];
      switch (refFunction.type) {
        case OperationTypes.AsyncApi:
          this.addFunctionFormGroup.get('properties')?.setValue(JSON.stringify(data.action.functionRef.arguments, null, '\t'));
          break;
        case OperationTypes.Rest:
          let properties = data.action.functionRef.arguments['properties'];
          let queries = data.action.functionRef.arguments['queries'];
          properties = properties ?? {};
          queries = queries ?? {};
          this.addFunctionFormGroup.get('properties')?.setValue(JSON.stringify(properties, null, '\t'));
          this.addFunctionFormGroup.get('queries')?.setValue(JSON.stringify(queries, null, '\t'));
          break;
      }
    }

    if (data.action.actionDataFilter) {
      this.addActionFormGroup.get('useResults')?.setValue(data.action.actionDataFilter.useResults.toString());
      this.addActionFormGroup.get('toStateData')?.setValue(data.action.actionDataFilter.toStateData);
      this.addActionFormGroup.get('results')?.setValue(data.action.actionDataFilter.results);
    }
  }

  private _generateFakeAsyncApiArguments(fn: StateMachineFunction) {
    const splitted = fn.operation?.split('#');
    if (splitted) {
      this.asyncApiService.getOperation(splitted[0], splitted[1]).subscribe((e) => {
        const payload = e.asyncApiOperation.message.payload;
        const components = e.components;
        let json: any = {};
        this._extractJsonFromComponent(payload['$ref'], json, components);
        this.addFunctionFormGroup.get('properties')?.setValue(JSON.stringify(json, undefined, 4));
      }, () => {

      });
    }
  }

  private _generateFakeOpenApiArguments(fn: StateMachineFunction) {
    const applicationJson = "application/json";
    const splitted = fn.operation?.split('#');
    if (splitted) {
      this.openApiService.getOperation(splitted[0], splitted[1]).subscribe((e) => {
        var queryParameters: any = {};
        var bodyParameters: any = {};
        const openApiOperation = e.openApiOperation;
        const components = e.components;
        if (openApiOperation.parameters) {
          openApiOperation.parameters.forEach((p: any) => {
            queryParameters = this._extractPathParameter(p, components);
          });
        }

        if (openApiOperation.requestBody && openApiOperation.requestBody.content && openApiOperation.requestBody.content[applicationJson]) {
          const prop = openApiOperation.requestBody.content[applicationJson];
          bodyParameters = this._extractPathParameter(prop, components);
        }

        this.addFunctionFormGroup.get('queries')?.setValue(JSON.stringify(queryParameters, undefined, 4));
        this.addFunctionFormGroup.get('properties')?.setValue(JSON.stringify(bodyParameters, undefined, 4));
      }, () => {

      });
    }
  }

  private _extractPathParameter(parameter: any, components: any, json: any = null, isArray: boolean = false) {
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
        } else {
          json[name].push(value);
        }
        break;
      case JSchemaStandardTypes.Integer:
        if (!isArray) {
          json[name] = 0;
        } else {
          json[name].push(0);
        }
        json[name] = 0;
        break;
      case JSchemaStandardTypes.Array:
        var arr: any = [];
        this._extractPathParameter(parameter.items, arr, components, true);
        if (!isArray) {
          json[name] = arr;
        } else {
          arr.forEach((r: any) => json.push(r));
        }
        break;
      default:
        const ref = parameter.schema["$ref"];
        this._extractJsonFromComponent(ref, json, components);
        break;
    }

    return json;
  }

  private _extractJsonFromComponent(reference: string, json: any, components: any) {
    const component = this._getComponent(reference, components);
    for (var key in component.properties) {
      const property = component.properties[key];
      this._extractJsonFromProperty(property, key, json, components, false);
    }
  }

  private _extractJsonFromProperty(property: any, propertyName: string, json: any, components: any, isArray: boolean = false) {
    var type = this._getJSchemaStandardType(property.type);
    switch (type) {
      case JSchemaStandardTypes.Integer:
        if (!isArray) {
          json[propertyName] = 0;
        } else {
          json.push(0);
        }
        break;
      case JSchemaStandardTypes.String:
        let str = this._getDefaultSchemaStrValue(property);
        if (!isArray) {
          json[propertyName] = str;
        } else {
          json.push(str);
        }
        break;
      case JSchemaStandardTypes.Array:
        var arr: any[] = [];
        this._extractJsonFromProperty(property.items, "", arr, components, true);
        if (!isArray) {
          json[propertyName] = arr;
        } else {
          arr.forEach((r) => json.push(r));
        }
        break;
      case JSchemaStandardTypes.Object:
        var obj: any = {};
        for (var key in property.properties) {
          const child = property.properties[key];
          this._extractJsonFromProperty(child, key, obj, components);
        }
        break;
      default:
        var obj: any = {};
        this._extractJsonFromComponent(property["$ref"], obj, components);
        if (!isArray) {
          json[propertyName] = obj;
        } else {
          json.push(obj);
        }
        break;
    }
  }

  private _getJSchemaStandardType(type: any) {
    if (Array.isArray(type)) {
      const filtered = type.filter((t: string) => JSchemaStandardTypes.All.filter(a => a === t).length > 0);
      return filtered[0];
    }

    return type;
  }

  private _getDefaultSchemaStrValue(schema: any) {
    if (schema.default) {
      return schema.default;
    }

    return "str";
  }

  private _getComponent(ref: string, components: any) {
    const splittedRef = ref.split('/');
    const name = splittedRef[splittedRef.length - 1];
    return components.schemas[name];
  }
}
