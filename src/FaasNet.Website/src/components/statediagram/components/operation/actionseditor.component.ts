import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { ActionDataFilter, OperationAction, OperationActionFunctionRef } from '@stores/statemachines/models/statemachine-operation-state.model';
import { OpenApiService } from '../../../../stores/openapi/services/openapi.service';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';
import { ExpressionEditorComponent } from '../expressioneditor/expressioneditor.component';

export class ActionsEditorData {
  functions: StateMachineFunction[] = [];
  actions: OperationAction[] = [];
}

@Component({
  selector: 'actionseditor',
  templateUrl: './actionseditor.component.html',
  styleUrls: [
    './actionseditor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class ActionsEditorComponent extends MatPanelContent {
  displayedColumns: string[] = ['actions', 'name', 'type'];
  functions: StateMachineFunction[] = [];
  actions: MatTableDataSource<OperationAction> = new MatTableDataSource<OperationAction>();
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
    private matDialog: MatDialog,
    private openApiService: OpenApiService) {
    super();
  }

  override init(data: any): void {
    this.functions = (data as ActionsEditorData).functions;
    this.actions.data = (data as ActionsEditorData).actions;
  }

  removeAction(index: number) {
    this.actions.data.splice(index, 1);
    this.actions.data = this.actions.data;
  }

  addAction() {
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

        let args : any = {};
        var queries = JSON.parse(this.addFunctionFormGroup.get('queries')?.value);
        var properties = JSON.parse(this.addFunctionFormGroup.get('properties')?.value);
        if (Object.keys(queries).length > 0) {
          args['queries'] = queries;
        }

        if (Object.keys(properties).length > 0) {
          args['properties'] = properties;
        }

        record.functionRef.arguments = args;
        break;
    }

    this.actions.data.push(record);
    this.actions.data = this.actions.data;
    this.actions = this.actions;
    this.addFunctionFormGroup.reset();
    this.addActionFormGroup.reset();
  }

  getType(action: OperationAction): string {
    if (action.functionRef) {
      return "function";
    }

    return "";
  }

  save() {
    this.onClosed.emit(this.actions.data);
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

  editResults() {
    const filter = this.addActionFormGroup.get('results')?.value;
    const dialogRef = this.matDialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r || !r.filter) {
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
      if (!r || !r.filter) {
        return;
      }

      this.addActionFormGroup.get('toStateData')?.setValue(r.filter);
    });
  }

  generateFakeOpenApiArguments(evt: any) {
    evt.preventDefault();
    const applicationJson = "application/json";
    const refName = this.addFunctionFormGroup.get('refName')?.value;
    const fn = this.functions.filter((f) => f.name === refName)[0];
    const splitted = fn.operation?.split('#');
    if (splitted) {
      this.openApiService.getOperation(splitted[0], splitted[1]).subscribe((e) => {
        var queryParameters: any = {};
        var bodyParameters: any = {};
        const openApiOperation = e.openApiOperation;
        const components = e.components;
        if (openApiOperation.parameters) {
          openApiOperation.parameters.forEach((p: any) => {
            queryParameters = this.extractPathParameter(p, components);
          });
        }

        if (openApiOperation.requestBody && openApiOperation.requestBody.content && openApiOperation.requestBody.content[applicationJson]) {
          const prop = openApiOperation.requestBody.content[applicationJson];
          bodyParameters = this.extractPathParameter(prop, components);
        }

        this.addFunctionFormGroup.get('queries')?.setValue(JSON.stringify(queryParameters, undefined, 4));
        this.addFunctionFormGroup.get('properties')?.setValue(JSON.stringify(bodyParameters, undefined, 4));
      }, () => {

      });
    }
  }

  isOpenApiUrl() {
    const refName = this.addFunctionFormGroup.get('refName')?.value;
    if (!refName) {
      return false;
    }

    const fn = this.functions.filter((f) => f.name === refName)[0];
    return fn.operation?.split('#').length === 2;
  }

  private extractPathParameter(parameter: any, components: any, json: any = null, isArray : boolean = false) {
    var type = parameter.schema["type"];
    if (!json) {
      json = {};
      if (type == "array") {
        json = [];
        isArray = true;
      }
    }

    var name = parameter.name;
    switch (type) {
      case "string":
        var value = this.getDefaultSchemaStrValue(parameter.schema);
        if (!isArray) {
          json[name] = value;
        } else {
          json[name].push(value);
        }
        break;
      case "integer":
        if (!isArray) {
          json[name] = 0;
        } else {
          json[name].push(0);
        }
        json[name] = 0;
        break;
      case "array":
        var arr: any = [];
        this.extractPathParameter(parameter.items, arr, components, true);
        if (!isArray) {
          json[name] = arr;
        } else {
          arr.forEach((r : any) => json.push(r));
        }
        break;
      default:
        const ref = parameter.schema["$ref"];
        this.extractJsonFromComponent(ref, json, components);
        break;
    }

    return json;
  }

  private extractJsonFromComponent(reference: string, json : any, components: any) {
    const component = this.getComponent(reference, components);
    for (var key in component.properties) {
      const property = component.properties[key];
      this.extractJsonFromProperty(property, key, json, components, false);
    }
  }

  private extractJsonFromProperty(property: any, propertyName: string, json: any, components: any, isArray : boolean = false) {
    switch (property.type) {
      case "integer":
        if (!isArray) {
          json[propertyName] = 0;
        } else {
          json.push(0);
        }
        break;
      case "string":
        let str = this.getDefaultSchemaStrValue(property);
        if (!isArray) {
          json[propertyName] = str;
        } else {
          json.push(str);
        }
        break;
      case "array":
        var arr: any[] = [];
        this.extractJsonFromProperty(property.items, "", arr, components, true);
        if (!isArray) {
          json[propertyName] = arr;
        } else {
          arr.forEach((r) => json.push(r));
        }
        break;
      case "object":
        var obj: any = {};
        for (var key in property.properties) {
          const child = property.properties[key];
          this.extractJsonFromProperty(child, key, obj, components);
        }
        break;
      default:
        var obj: any = {};
        this.extractJsonFromComponent(property["$ref"], obj, components);
        if (!isArray) {
          json[propertyName] = obj;
        } else {
          json.push(obj);
        }
        break;
    }
  }

  private getDefaultSchemaStrValue(schema: any) {
    if (schema.default) {
      return schema.default;
    }

    return "str";
  }

  private getComponent(ref: string, components: any) {
    const splittedRef = ref.split('/');
    const name = splittedRef[splittedRef.length - 1];
    return components.schemas[name];
  }
}
