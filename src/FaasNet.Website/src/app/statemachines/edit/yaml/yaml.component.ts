import { Component, Input } from '@angular/core';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { Document } from 'yaml';

@Component({
  selector: 'display-state-machine-yaml',
  templateUrl: './yaml.component.html',
  styleUrls: [
    './yaml.component.scss',
  ]
})
export class YamlComponent {
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  yaml: string = "";
  yamlOptions = { theme: 'vs', language: 'yaml' };
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    const doc = new Document();
    doc.contents = this._stateMachineModel.getJson();
    this.yaml = doc.toString();
  }
}
