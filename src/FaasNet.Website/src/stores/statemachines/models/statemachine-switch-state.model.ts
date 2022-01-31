import { BehaviorSubject } from "rxjs";
import { BaseTransition, EmptyTransition, StateMachineState } from "./statemachine-state.model";

export class EventCondition extends BaseTransition {
  static TYPE: string = "event";
  private _eventRef: string = "";
  private label: BehaviorSubject<string> | undefined;
  get eventRef() {
    return this._eventRef;
  }
  set eventRef(str: string) {
    this._eventRef = str;
    this.label?.next(this._eventRef);
  }

  constructor() {
    super();
    this.label = new BehaviorSubject<string>("");
  }

  public override getType(): string {
    return EventCondition.TYPE;
  }

  public override getLabel(): BehaviorSubject<string> | undefined {
    return new BehaviorSubject("event");
  }

  public getJson(): any {
    return {
      eventRef: this.eventRef,
      transition: this.transition
    };
  }

  public static build(json: any) {
    var result = new EventCondition();
    result.eventRef = json["eventRef"];
    result.label = json["label"];
    result.transition = json["transition"];
    return result;
  }
}

export class DataCondition extends BaseTransition {
  static TYPE: string = "data";
  private _condition: string = "";
  name: string | undefined;
  get condition() : string {
    return this._condition;
  }
  set condition(str: string) {
    this._condition = str;
    this.label?.next(this._condition);
  }
  private label: BehaviorSubject<string> | undefined;

  constructor() {
    super();
    this.label = new BehaviorSubject<string>("");
  }

  public override getType(): string {
    return DataCondition.TYPE;
  }

  public override getLabel(): BehaviorSubject<string> | undefined {
    return this.label;
  }

  public getJson(): any {
    var result : any = {
      name: this.name,
      transition: this.transition
    };
    if (this.condition) {
      result["condition"] = this.condition;
    }

    return result;
  }

  public static build(json: any) {
    var result = new DataCondition();
    if (json["condition"]) {
      result.condition = json["condition"];
    }

    result.condition = json["condition"];
    result.name = json["name"];
    result.transition = json["transition"];
    return result;
  }
}

export class SwitchStateMachineState extends StateMachineState {
  public static TYPE: string = "switch";
  public static EVENT_TYPE: string = "event";
  public static DATA_TYPE: string = "data";

  constructor() {
    super();
    this.type = SwitchStateMachineState.TYPE;
    this.eventConditions = [];
    this.dataConditions = [];
  }

  switchType: string = "event";
  eventConditions: EventCondition[];
  dataConditions: DataCondition[] = [];
  defaultCondition: BaseTransition | null = null;

  public override setTransitions(transitions: BaseTransition[]) {
    if (transitions) {
      switch (this.switchType) {
        case SwitchStateMachineState.EVENT_TYPE:
          this.eventConditions = transitions.filter((t: BaseTransition) => {
            return t.getType() == EventCondition.TYPE;
          }).map((t: BaseTransition) => {
            return t as EventCondition;
          });
          break;
        case SwitchStateMachineState.DATA_TYPE:
          this.dataConditions = transitions.filter((t: BaseTransition) => {
            return t.getType() == DataCondition.TYPE;
          }).map((t: BaseTransition) => {
            return t as DataCondition;
          });
          break;
      }
    }

    this.end = transitions.length === 0;
    return [];
  }

  public override tryAddTransition(transitionName: string): string | null {
    this.end = false;
    switch (this.switchType) {
      case SwitchStateMachineState.EVENT_TYPE:
        const newEvtTransition = new EventCondition();
        newEvtTransition.eventRef = "event";
        newEvtTransition.transition = transitionName;
        this.eventConditions.push(newEvtTransition);
        break;
      case SwitchStateMachineState.DATA_TYPE:
        const newDataCondition = new DataCondition();
        newDataCondition.condition = "true";
        newDataCondition.transition = transitionName;
        this.dataConditions.push(newDataCondition);
        break;
    }

    return null;
  }

  public override getNextTransitions(): BaseTransition[] {
    let arr: BaseTransition[] = [];
    this.eventConditions.forEach((ec) => {
      arr.push(ec);
    });

    this.dataConditions.forEach((dc) => {
      arr.push(dc);
    });

    if (this.defaultCondition && this.defaultCondition.transition) {
      arr.push(this.defaultCondition);
    }

    return arr;
  }

  public switchToEventCondition() : void {
    this.switchType = SwitchStateMachineState.EVENT_TYPE;
    this.eventConditions = this.dataConditions.map((e: DataCondition) => {
      const record = new EventCondition();
      record.eventRef = "event";
      record.transition = e.transition;
      return record;
    });
    this.dataConditions = [];
  }

  public switchToDataCondition(): void {
    this.switchType = SwitchStateMachineState.DATA_TYPE;
    this.dataConditions = this.eventConditions.map((e: EventCondition) => {
      const record = new DataCondition();
      record.transition = e.transition;
      record.condition = "true";
      return record;
    });
    this.eventConditions = [];
  }

  public swichTransitionToDefault(transition: BaseTransition): void {
    if (this.defaultCondition) {
      if (transition.transition == this.defaultCondition.transition) {
        return;
      }

      this.tryAddTransition(this.defaultCondition.transition);
    }

    this.removeTransition(transition);
    this.defaultCondition = new EmptyTransition();
    this.defaultCondition.transition = transition.transition;
  }

  public override getJson() {
    var result : any = {
      id: this.id,
      name: this.name,
      type: SwitchStateMachineState.TYPE,
      end: this.end,
      dataConditions: this.dataConditions.map((d: DataCondition) => {
        return d.getJson()
      })
    };

    if (this.defaultCondition && this.defaultCondition.transition) {
      result.defaultCondition = {
        transition: this.defaultCondition.transition
      };
    }

    if (this.stateDataFilter) {
      result.stateDataFilter = this.stateDataFilter.getJson();
    }

    if (this.eventConditions && this.eventConditions.length > 0) {
      result.eventConditions = this.eventConditions.map((e: EventCondition) => {
        return e.getJson();
      });
    }

    if (this.dataConditions && this.dataConditions.length > 0) {
      result.dataConditions = this.dataConditions.map((d: DataCondition) => {
        return d.getJson()
      });
    }

    return result;
  }

  private removeTransition(transition: BaseTransition) {
    switch (this.switchType) {
      case SwitchStateMachineState.EVENT_TYPE:
        {
          const index = this.eventConditions.indexOf(transition as EventCondition);
          this.eventConditions.splice(index, 1);
        }
        break;
      case SwitchStateMachineState.DATA_TYPE:
        {
          const index = this.dataConditions.indexOf(transition as DataCondition);
          this.dataConditions.splice(index, 1);
        }
        break;
    }
  }
}

export class DefaultCondition {
  transition: string | undefined;
}
