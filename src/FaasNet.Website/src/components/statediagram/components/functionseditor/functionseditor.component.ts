import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { StateMachineFunction } from '@stores/statemachines/models/statemachine-function.model';
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
  functionIndex: number = 0;
  editFunctionFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required]),
    operation: new FormControl(),
  });
  edit: boolean = false;
  editKubernetesFormGroup: FormGroup = new FormGroup({
    image: new FormControl('', [Validators.required]),
    version: new FormControl('', [Validators.required]),
    configuration: new FormControl()
  });
  displayedColumns: string[] = ['actions', 'name', 'type' ];

  constructor() {
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
    record.operation = this.editFunctionFormGroup.get('operation')?.value;
    let type = this.editFunctionFormGroup.get('type')?.value;
    if (type === 'kubernetes') {
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
    }

    record.type = type;
    if (!this.edit) {
      this.functions.data.push(record);
    }

    this.functions.data = this.functions.data;
    this.editFunctionFormGroup.reset();
    this.editKubernetesFormGroup.reset();
    this.edit = false;
  }

  editFunction(fn: StateMachineFunction, index: number) {
    this.editFunctionFormGroup.reset();
    this.editKubernetesFormGroup.reset();
    this.functionIndex = index;
    this.edit = true;
    this.editFunctionFormGroup.get('name')?.setValue(fn.name);
    this.editFunctionFormGroup.get('operation')?.setValue(fn.operation);
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

    return false;
  }

  save() {
    this.onClosed.emit(this.functions.data);
  }
}
