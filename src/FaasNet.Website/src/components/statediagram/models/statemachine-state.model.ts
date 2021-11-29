export abstract class StateMachineState {
  name: string | undefined;
  type: string | undefined;

  public abstract setTransitions(transitions: string[]): string[];
  public abstract getNextTransitions(): BaseTransition[];
}

export abstract class BaseTransition {
  transition: string = "";
  public abstract getType(): string;
  public abstract getLabel(): string | undefined;
}

export class EmptyTransition extends BaseTransition {
  static TYPE: string = "empty";
  public override getType(): string {
    return EmptyTransition.TYPE;
  }

  public override getLabel(): string | undefined {
    return undefined;
  }
}

export class FlowableStateMachineState extends StateMachineState{
  constructor() {
    super();
    this.transition = "";
  }

  transition: string;

  public override setTransitions(transitions: string[]): string[] {
    const oldTransition = this.transition;
    if (transitions && transitions.length > 0) {
      this.transition = transitions[0];
    }

    return oldTransition == "" ? [] : [oldTransition];
  }

  public override getNextTransitions(): BaseTransition[] {
    const record: EmptyTransition = new EmptyTransition();
    record.transition = this.transition;
    return [
      record
    ];
  }
}
