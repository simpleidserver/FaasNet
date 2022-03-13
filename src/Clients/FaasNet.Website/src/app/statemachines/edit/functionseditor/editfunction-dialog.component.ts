import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { AsyncApiService } from "@stores/asyncapi/services/asyncapi.service";
import { OpenApiService } from "@stores/openapi/services/openapi.service";
import { OperationTypes } from "@stores/statemachines/models/operation-types.model";
import { StateMachineFunction } from "@stores/statemachines/models/statemachine-function.model";

@Component({
  selector: 'edit-function-dialog-dialog',
  templateUrl: './editfunction-dialog.component.html'
})
export class EditFunctionDialogComponent {
  editFunctionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required])
  });
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
  openApiErrorMessage: string | null = null;
  asyncApiErrorMessage: string | null = null;
  operations: any[] = [];
  asyncApiOperations: any[] = [];
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

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: StateMachineFunction,
    private dialogRef: MatDialogRef<EditFunctionDialogComponent>,
    private openApiService: OpenApiService,
    private asyncApiService: AsyncApiService) {
    this._init(data);
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
    if (type === OperationTypes.Rest && isOpenApiUrl) {
      return !this.editRestFormGroup.valid;
    }

    if (type === OperationTypes.Rest && !isOpenApiUrl) {
      return this.editRestFormGroup.get('url')?.errors?.required;
    }

    if (type === OperationTypes.AsyncApi && !this.editAsyncApiFormGroup.valid) {
      return true;
    }

    return false;
  }

  save() {
    if (this.isDisabled()) {
      return;
    }

    var record = new StateMachineFunction();
    record.name = this.editFunctionFormGroup.get('name')?.value;
    let type = this.editFunctionFormGroup.get('type')?.value;
    switch (type) {
      case 'kubernetes':
        type = OperationTypes.Custom;
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
      case OperationTypes.Rest:
        var url = this.editRestFormGroup.get('url')?.value;
        const isOpenApiUrl = this.editRestFormGroup.get('isOpenApiUrl')?.value;
        if (isOpenApiUrl) {
          url = url + '#' + this.editRestFormGroup.get('operationId')?.value;
        }

        record.operation = url;
        break;
      case OperationTypes.AsyncApi:
        var url = this.editAsyncApiFormGroup.get('url')?.value;
        url = url + '#' + this.editAsyncApiFormGroup.get('operationId')?.value;
        record.operation = url;
        break;
    }

    record.type = type;
    this.dialogRef.close(record);
  }

  extractOpenApi(evt: any) {
    evt.preventDefault();
    this.editRestFormGroup.get('operationId')?.setValue('');
    this._refreshOpenApiOperations();
  }

  extractAsyncApi(evt: any) {
    evt.preventDefault();
    this.editAsyncApiFormGroup.get('operationId')?.setValue('');
    this._refreshAsyncApiOperations();
  }

  displaySelectedOpenApiOperation() {
    const operationId = this.editRestFormGroup.get('operationId')?.value;
    const filtered = this.operations.filter((o) => o.operationId == operationId);
    if (filtered.length !== 1) {
      return "";
    }

    return "Summary: " + filtered[0].summary;
  }

  displaySelectedAsyncApiOperation() {
    const operationId = this.editAsyncApiFormGroup.get('operationId')?.value;
    const filtered = this.asyncApiOperations.filter((o) => o.operationId == operationId);
    if (filtered.length !== 1) {
      return "";
    }

    return "Summary: " + filtered[0].summary;
  }

  private _init(data : StateMachineFunction) {
    if (!data) {
      return;
    }

    this.editFunctionFormGroup.get('name')?.setValue(data.name);
    this.editFunctionFormGroup.get('type')?.setValue(data.type);
    if (data.metadata) {
      if (data.metadata.version && data.metadata.image) {
        this.editFunctionFormGroup.get('type')?.setValue('kubernetes');
        this.editKubernetesFormGroup.get('version')?.setValue(data.metadata.version);
        this.editKubernetesFormGroup.get('image')?.setValue(data.metadata.image);
        if (data.metadata.configuration) {
          this.editKubernetesFormGroup.get('configuration')?.setValue(JSON.stringify(data.metadata.configuration));
        }
      }
    }

    if (data.type === OperationTypes.Rest) {
      const splittedOperation = data.operation?.split('#');
      let isOpenApiUrl = false;
      let url = data.operation;
      if (splittedOperation?.length === 2) {
        url = splittedOperation[0];
        isOpenApiUrl = true;
        const operationId = splittedOperation[1];
        this.editRestFormGroup.get('operationId')?.setValue(operationId);
      }

      this.editRestFormGroup.get('url')?.setValue(url);
      this.editRestFormGroup.get('isOpenApiUrl')?.setValue(isOpenApiUrl);
      if (isOpenApiUrl) {
        this._refreshOpenApiOperations();
      }
    }

    if (data.type === OperationTypes.AsyncApi) {
      const splittedOperation = data.operation?.split('#');
      if (splittedOperation) {
        const url = splittedOperation[0];
        const operationId = splittedOperation[1];
        this.editAsyncApiFormGroup.get('operationId')?.setValue(operationId);
        this.editAsyncApiFormGroup.get('url')?.setValue(url);
        this._refreshAsyncApiOperations();
      }
    }
  }

  private _refreshOpenApiOperations() {
    const url = this.editRestFormGroup.get('url')?.value;
    this.operations = [{ 'operationId': 'Loading...' }]
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

  private _refreshAsyncApiOperations() {
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
