import { Component, OnDestroy, OnInit } from '@angular/core';
import { InjectStateMachineState } from '../../../components/statediagram/models/statemachine-inject-state.model';
import { EventCondition, SwitchStateMachineState } from '../../../components/statediagram/models/statemachine-switch-state.model';
import { StateMachine } from '../../../components/statediagram/models/statemachine.model';

@Component({
  selector: 'edit-state-machine',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditStateMachineComponent implements OnInit, OnDestroy {
  stateMachineDef: StateMachine | null = null;

  constructor() {
    this.stateMachineDef = new StateMachine();
    this.stateMachineDef.id = "id";
    this.stateMachineDef.specVersion = "1.0";
    this.stateMachineDef.start = "start";
    this.stateMachineDef.version = "1.0";
    const switchState = new SwitchStateMachineState();
    switchState.id = "1";
    switchState.name = "switch";
    const firstEvtCondition = new EventCondition();
    firstEvtCondition.eventRef = "evt1";
    firstEvtCondition.transition = "2";
    const secondEvtCondition = new EventCondition();
    secondEvtCondition.eventRef = "evt2";
    secondEvtCondition.transition = "3";
    switchState.eventConditions = [firstEvtCondition, secondEvtCondition];
    const firstInject = new InjectStateMachineState();
    firstInject.id = "2";
    firstInject.name = "firstInject";
    const secondInject = new InjectStateMachineState();
    secondInject.id = "3";
    secondInject.name = "secondInject";
    secondInject.transition = "4";
    const thirdInject = new InjectStateMachineState();
    thirdInject.id = "4";
    thirdInject.name = "thirdInject";
    this.stateMachineDef.states.push(switchState);
    this.stateMachineDef.states.push(firstInject);
    this.stateMachineDef.states.push(secondInject);
    this.stateMachineDef.states.push(thirdInject);

  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }
}
