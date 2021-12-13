import { FlowableStateMachineState, StateDataFilter } from "./statemachine-state.model";

export class InjectStateMachineState extends FlowableStateMachineState {
  public static TYPE: string = "inject";
  public data: any | null = null;
  constructor() {
    super();
    this.type = InjectStateMachineState.TYPE;
    this.stateDataFilter = new StateDataFilter();
  }

  public override getJson() {
    let result : any = {
      id: this.id,
      name: this.name,
      stateDataFilter: this.stateDataFilter?.getJson(),
      transition: this.transition,
      type: InjectStateMachineState.TYPE,
      end: this.end
    };
    if (this.data && Object.keys(this.data).length > 0) {
      result['data'] = this.data;
    }

    return result;
  }
}
