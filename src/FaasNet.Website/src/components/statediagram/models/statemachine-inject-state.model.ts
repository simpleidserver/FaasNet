import { FlowableStateMachineState } from "./statemachine-state.model";

export class InjectStateMachineState extends FlowableStateMachineState {
  public static TYPE: string = "inject";
  constructor() {
    super();
    this.type = InjectStateMachineState.TYPE;
  }
}
