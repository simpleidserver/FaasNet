import { FlowableStateMachineState } from "./statemachine-state.model";

export class InjectStateMachineState extends FlowableStateMachineState{
  constructor() {
    super();
    this.type = 'inject';
  }
}
