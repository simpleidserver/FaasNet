import { Component, OnDestroy, OnInit } from '@angular/core';
import { InjectStateMachineState } from '../../../components/statediagram/models/statemachine-inject-state.model';
import { StateMachine } from '../../../components/statediagram/models/statemachine.model';

@Component({
  selector: 'edit-state-machine',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditStateMachineComponent implements OnInit, OnDestroy {
  stateMachineDef: StateMachine = {
    id: "id",
    specVersion: "1.0",
    start: "start",
    version: "1.0",
    states: []
  };

  constructor() {
    const firstInject: InjectStateMachineState = { name: "firstInject", type: "inject", transition: "secondInject" };
    const secondInject: InjectStateMachineState = { name: "secondInject", type: "inject", transition: "" };
    this.stateMachineDef.states.push(firstInject);
    this.stateMachineDef.states.push(secondInject);

  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }
}
