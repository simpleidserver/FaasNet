import { EventEmitter } from "@angular/core";
import { BehaviorSubject } from "rxjs";

export abstract class StateMachineState {
  id: string | undefined;
  name: string | undefined;
  type: string | undefined;
  end: boolean = false;
  stateDataFilter: StateDataFilter | undefined;

  public abstract setTransitions(transitions: BaseTransition[]): void;
  public abstract tryAddTransition(transitionName: string): string | null;
  public abstract getNextTransitions(): BaseTransition[];
  public abstract getJson(): any;
  public updated: EventEmitter<StateMachineState> = new EventEmitter<StateMachineState>();
}

export class StateDataFilter {
  input: string = "";
  output: string = "";

  public getJson() {
    var result: any = {};
    if (this.input) {
      result["input"] = this.input;
    }

    if (this.output) {
      result["output"] = this.output;
    }

    return result;
  }

  public static build(json: any): StateDataFilter | undefined {
    if (!json) {
      return undefined;
    }

    var result = new StateDataFilter();
    if (json["input"]) {
      result.input = json["input"];
    }

    if (json["output"]) {
      result.output = json["output"];
    }

    return result;
  }
}

export abstract class BaseTransition {
  transition: string = "";
  public abstract getType(): string;
  public abstract getLabel(): BehaviorSubject<string> | undefined;
}

export class EmptyTransition extends BaseTransition {
  static TYPE: string = "empty";
  private label: BehaviorSubject<string> | undefined;

  constructor() {
    super();
    this.label = new BehaviorSubject<string>("transition");
  }

  public override getType(): string {
    return EmptyTransition.TYPE;
  }

  public override getLabel(): BehaviorSubject<string> | undefined {
    return this.label;
  }

  public static build(json: any) {
    var result = new EmptyTransition();
    result.label = json["label"];
    result.transition = json["transition"];
    return result;
  }
}

export abstract class FlowableStateMachineState extends StateMachineState{
  constructor() {
    super();
    this.transition = "";
  }

  transition: string;

  public override setTransitions(transitions: BaseTransition[]): void {
    if (transitions.length > 0) {
      this.transition = transitions[0].transition;
      this.end = false;
    } else {
      this.transition = "";
      this.end = true;
    }
  }

  public override tryAddTransition(transitionName: string): string | null{
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
}
