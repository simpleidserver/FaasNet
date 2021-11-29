import { StateMachineState, BaseTransition } from "./statemachine-state.model";

export class EventCondition extends BaseTransition {
  static TYPE : string = "event";
  constructor() {
    super();
  }

  eventRef: string | undefined;
  public override getType(): string {
    return EventCondition.TYPE;
  }

  public override getLabel(): string | undefined {
    return this.eventRef;
  }
}

export class DataCondition extends BaseTransition {
  static TYPE: string = "data";
  name: string | undefined;
  condition: string | undefined;

  public override getType(): string {
    return DataCondition.TYPE;
  }

  public override getLabel(): string | undefined {
    return this.condition;
  }
}

export class SwitchStateMachineState extends StateMachineState {
  public static TYPE: string = "switch";

  constructor() {
    super();
    this.eventConditions = [];
    this.dataConditions = [];
  }

  eventConditions: EventCondition[];
  dataConditions: DataCondition[] = [];
  defaultCondition: BaseTransition | null = null;

  public override setTransitions(transitions: string[]) : string[] {
    if (transitions) {
      transitions.forEach((t: string) => {
        const evtCondition = new EventCondition();
        evtCondition.eventRef = "";
        evtCondition.transition = t;
        this.eventConditions.push(evtCondition);
      });
    }

    return [];
  }

  public override getNextTransitions(): BaseTransition[] {
    let arr: BaseTransition[] = [];
    if (this.defaultCondition && this.defaultCondition.transition) {
      arr.push(this.defaultCondition);
    }

    this.eventConditions.forEach((ec) => {
      arr.push(ec);
    });

    this.dataConditions.forEach((dc) => {
      arr.push(dc);
    });

    return arr;
  }
}

export class DefaultCondition {
  transition: string | undefined;
}
