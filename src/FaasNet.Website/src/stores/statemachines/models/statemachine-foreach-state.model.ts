import { BaseTransition, EmptyTransition, StateMachineState } from "./statemachine-state.model";

export class ForeachStateMachineState extends StateMachineState {
  public static TYPE: string = "foreach";
  transition: string;

  constructor() {
    super();
    this.type = ForeachStateMachineState.TYPE;
    this.transition = "";
  }


  public override setTransitions(transitions: BaseTransition[]) : void {
    if (transitions.length > 0) {
      this.transition = transitions[0].transition;
      this.end = false;
    } else {
      this.end = true;
      this.transition = "";
    }
  }

  public override tryAddTransition(transitionName: string): string | null {
    const oldTransition = this.transition;
    this.end = false;
    this.transition = transitionName;
    return oldTransition;
  }

  public override getNextTransitions(): BaseTransition[] {
    const record: EmptyTransition = new EmptyTransition();
    record.transition = this.transition;
    return [
      record
    ];
  }

  public override getJson() {
    var result = {
      id: this.id,
      name: this.name,
      stateDataFilter: this.stateDataFilter?.getJson(),
      transition: this.transition,
      type: ForeachStateMachineState.TYPE,
      end: this.end
    };
    if (this.stateDataFilter) {
      result['stateDataFilter'] = this.stateDataFilter.getJson();
    }

    return result;
  }
}
