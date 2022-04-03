import { Component, Input } from '@angular/core';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';

@Component({
  selector: 'display-state-machine-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.scss']
})
export class StateMachineInfoComponent {
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
  }
}
