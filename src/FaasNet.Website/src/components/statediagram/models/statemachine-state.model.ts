export class StateMachineState {
  name: string | undefined;
  type: string | undefined;
}

export class FlowableStateMachineState extends StateMachineState{
  constructor() {
    super();
    this.transition = "";
  }

  transition: string;
}
