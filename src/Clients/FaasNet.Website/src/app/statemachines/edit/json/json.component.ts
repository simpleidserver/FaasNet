import { Component, Input } from '@angular/core';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';

@Component({
  selector: 'display-state-machine-json',
  templateUrl: './json.component.html',
  styleUrls: [
    './json.component.scss',
  ]
})
export class JsonComponent {
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  json: string = "";
  jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    this.json = JSON.stringify(this._stateMachineModel.getJson(), null, "\t");
  }
}
