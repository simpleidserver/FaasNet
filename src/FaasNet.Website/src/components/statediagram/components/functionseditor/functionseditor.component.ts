import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
import { AsyncApiService } from '../../../../stores/asyncapi/services/asyncapi.service';
import { OpenApiService } from '../../../../stores/openapi/services/openapi.service';
import { MatPanelComponent } from '../../../matpanel/matpanel.component';
import { MatPanelContent } from '../../../matpanel/matpanelcontent';

export class FunctionsEditorData {
  functions: StateMachineFunction[] = [];
}

@Component({
  selector: 'functionseditor',
  templateUrl: './functionseditor.component.html',
  styleUrls: [
    './functionseditor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class FunctionsEditorComponent extends MatPanelContent {
  functions: MatTableDataSource<StateMachineFunction> = new MatTableDataSource<StateMachineFunction>();
  panel: MatPanelComponent | null = null;
  operations: any[] = [];
  asyncApiErrorMessage: string | null = null;
  asyncApiOperations: any[] = [];
  functionIndex: number = 0;
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
  openApiErrorMessage: string | null = null;
  editFunctionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required])
  });
  edit: boolean = false;
  editRestFormGroup: FormGroup = new FormGroup({
    url: new FormControl('', [Validators.required]),
    isOpenApiUrl: new FormControl(false),
    operationId: new FormControl('', [Validators.required])
  });
  editAsyncApiFormGroup: FormGroup = new FormGroup({
    url: new FormControl('', [Validators.required]),
    operationId: new FormControl('', [Validators.required])
  });
  editKubernetesFormGroup: FormGroup = new FormGroup({
    image: new FormControl('', [Validators.required]),
    version: new FormControl('', [Validators.required]),
    configuration: new FormControl()
  });
  displayedColumns: string[] = ['actions', 'name', 'type' ];

  constructor(private openApiService: OpenApiService, private asyncApiService : AsyncApiService) {
    super();
  }

  override init(data: any): void {
    this.functions.data = (data as FunctionsEditorData).functions;
  }

  deleteFunction(i: number) {
    this.functions.data.splice(i, 1);
    this.functions.data = this.functions.data;
    this.editFunctionFormGroup.reset();
    this.editKubernetesFormGroup.reset();
    this.edit = false;
  }

  addFunction() {
    if (this.isDisabled()) {
      return;
    }

    var record = new StateMachineFunction();
    if (this.edit) {
      record = this.functions.data[this.functionIndex];
    }

    record.name = this.editFunctionFormGroup.get('name')?.value;
    let type = this.editFunctionFormGroup.get('type')?.value;
    switch (type) {
      case 'kubernetes':
        type = 'custom';
        record.metadata = {
          version: this.editKubernetesFormGroup.get('version')?.value,
          image: this.editKubernetesFormGroup.get('image')?.value
        };
        var configuration = this.editKubernetesFormGroup.get('configuration')?.value;
        if (configuration) {
          try {
            var o = JSON.parse(configuration);
            record.metadata['configuration'] = o;
          } catch { }
        }
        break;
      case 'rest':
        var url = this.editRestFormGroup.get('url')?.value;
        const isOpenApiUrl = this.editRestFormGroup.get('isOpenApiUrl')?.value;
        if (isOpenApiUrl) {
          url = url + '#' + this.editRestFormGroup.get('operationId')?.value;
        }

        record.operation = url;
        break;
      case 'asyncapi':
        var url = this.editAsyncApiFormGroup.get('url')?.value;
        url = url + '#' + this.editAsyncApiFormGroup.get('operationId')?.value;
        record.operation = url;
        break;
    }

    record.type = type;
    if (!this.edit) {
      this.functions.data.push(record);
    }

    this.functions.data = this.functions.data;
    this.editFunctionFormGroup.reset();
    this.editKubernetesFormGroup.reset();
    this.editRestFormGroup.reset();
    this.edit = false;
  }

  editFunction(fn: StateMachineFunction, index: number) {
    this.editFunctionFormGroup.reset();
    this.editKubernetesFormGroup.reset();
    this.editRestFormGroup.reset();
    this.functionIndex = index;
    this.edit = true;
    this.editFunctionFormGroup.get('name')?.setValue(fn.name);
    this.editFunctionFormGroup.get('type')?.setValue(fn.type);
    if (fn.metadata) {
      if (fn.metadata.version && fn.metadata.image) {
        this.editFunctionFormGroup.get('type')?.setValue('kubernetes');
        this.editKubernetesFormGroup.get('version')?.setValue(fn.metadata.version);
        this.editKubernetesFormGroup.get('image')?.setValue(fn.metadata.image);
        if (fn.metadata.configuration) {
          this.editKubernetesFormGroup.get('configuration')?.setValue(JSON.stringify(fn.metadata.configuration));
        }
      }
    }

    if (fn.type === 'rest') {
      const splittedOperation = fn.operation?.split('#');
      let isOpenApiUrl = false;
      let url = fn.operation;
      if (splittedOperation?.length === 2) {
        url = splittedOperation[0];
        isOpenApiUrl = true;
        const operationId = splittedOperation[1];
        this.editRestFormGroup.get('operationId')?.setValue(operationId);
      }

      this.editRestFormGroup.get('url')?.setValue(url);
      this.editRestFormGroup.get('isOpenApiUrl')?.setValue(isOpenApiUrl);
      if (isOpenApiUrl) {
        this.refreshOpenApiOperations();
      }
    }

    if (fn.type === 'asyncapi') {
      const splittedOperation = fn.operation?.split('#');
      if (splittedOperation) {
        const url = splittedOperation[0];
        const operationId = splittedOperation[1];
        this.editAsyncApiFormGroup.get('operationId')?.setValue(operationId);
        this.editAsyncApiFormGroup.get('url')?.setValue(url);
        this.refreshAsyncApiOperations();
      }
    }
  }

  isSelected(i: number) {
    if (!this.edit) {
      return false;
    }

    return this.functionIndex == i;
  }

  getOperationTranslationKey() {
    let type = this.editFunctionFormGroup.get('type')?.value;
    switch (type) {
      case 'rest':
        return 'API url or OPENAPI url';
    }

    return '';
  }

  isDisabled() {
    if (!this.editFunctionFormGroup.valid) {
      return true;
    }

    let type = this.editFunctionFormGroup.get('type')?.value;
    if (type == 'kubernetes') {
      return !this.editKubernetesFormGroup.valid;
    }

    const isOpenApiUrl = this.editRestFormGroup.get('isOpenApiUrl')?.value;
    if (type === 'rest' && isOpenApiUrl) {
      return !this.editRestFormGroup.valid;
    }

    if (type === 'rest' && !isOpenApiUrl) {
      return this.editRestFormGroup.get('url')?.errors?.required;
    }

    if (type === 'asyncapi' && !this.editAsyncApiFormGroup.valid) {
      return true;
    }

    return false;
  }

  extract(evt: any) {
    evt.preventDefault();
    this.editRestFormGroup.get('operationId')?.setValue('');
    this.refreshOpenApiOperations();
  }

  extractAsyncApi(evt: any) {
    evt.preventDefault();
    this.editAsyncApiFormGroup.get('operationId')?.setValue('');
    this.refreshAsyncApiOperations();
  }

  save() {
    this.onClosed.emit(this.functions.data);
  }

  displaySelectedOperation() {
    const operationId = this.editRestFormGroup.get('operationId')?.value;
    const filtered = this.asyncApiOperations.filter((o) => o.operationId == operationId);
    if (filtered.length !== 1) {
      return "";
    }

    return "Summary: " + filtered[0].summary;
  }

  displaySelectedAsyncOperation() {
    const operationId = this.editAsyncApiFormGroup.get('operationId')?.value;
    const filtered = this.asyncApiOperations.filter((o) => o.operationId == operationId);
    if (filtered.length !== 1) {
      return "";
    }

    return "Summary: " + filtered[0].summary;
  }

  private refreshOpenApiOperations() {
    const url = this.editRestFormGroup.get('url')?.value;
    this.operations = [{ 'operationId' : 'Loading...' }]
    this.openApiService.getOperations(url).subscribe((e) => {
      this.openApiErrorMessage = null;
      this.operations = e;
      this.editRestFormGroup.get('operationId')?.setValue(e[0].operationId);
    }, (e) => {
      this.operations = [];
      if (e.error && e.error.Message) {
        this.openApiErrorMessage = e.error.Message;
      } else {
        this.openApiErrorMessage = "An error occured while trying to get the operations from the OPENAPI URL";
      }
    });
  }

  private refreshAsyncApiOperations() {
    const url = this.editAsyncApiFormGroup.get('url')?.value;
    this.asyncApiOperations = [{ 'operationId': 'Loading...' }];
    this.asyncApiService.getOperations(url).subscribe((e) => {
      this.asyncApiErrorMessage = null;
      this.asyncApiOperations = e;
      this.editAsyncApiFormGroup.get('operationId')?.setValue(e[0].operationId);
    }, (e) => {
      if (e.error && e.error.Message) {
        this.asyncApiErrorMessage = e.error.Message;
      } else {
        this.asyncApiErrorMessage = "An error occured while trying to get the operations from the ASYNCAPI URL";
      }
    });
    
  }
}
