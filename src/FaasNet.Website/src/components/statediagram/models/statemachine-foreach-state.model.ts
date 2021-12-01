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
    if (transitions && transitions.length > 0) {
      this.transition = transitions[0].transition;
    }
  }

  public override tryAddTransition(transitionName: string): string | null {
    const oldTransition = this.transition;
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
}
