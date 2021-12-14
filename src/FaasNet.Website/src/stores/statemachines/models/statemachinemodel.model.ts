import { InjectStateMachineState } from "./statemachine-inject-state.model";
import { BaseTransition, EmptyTransition, StateDataFilter, StateMachineState } from "./statemachine-state.model";
import { DataCondition, DefaultCondition, EventCondition, SwitchStateMachineState } from "./statemachine-switch-state.model";

export class StartStateMachineModel {
  stateName: string | undefined;
  schedule: string | undefined;

  public getJson() {
    var result: any = {};
    if (this.stateName) {
      result['stateName'] = this.stateName;
    }

    if (this.schedule) {
      result['schedule'] = this.schedule;
    }

    return result;
  }

  public static build(json: any): StartStateMachineModel{
    var result = new StartStateMachineModel();
    if (json['stateName']) {
      result.stateName = json['stateName'];
    }

    if (json['schedule']) {
      result.schedule = json['schedule'];
    }

    return result;
  }
}

export class StateMachineModel {
  constructor() {
    this.states = [];
  }

  id: string | undefined;
  name: string | undefined;
  description: string | undefined;
  version: string | undefined;
  specVersion: string | undefined;
  start: StartStateMachineModel | undefined;
  states: StateMachineState[];

  get isEmpty() {
    return this.states.length === 0;
  }

  public remove(state: StateMachineState): void {
    const self = this;
    const nextTransitions = state.getNextTransitions();
    nextTransitions.forEach((t: BaseTransition) => {
      const child = self.getState(t.transition);
      if (child) {
        self.remove(child);
      }
    });

    const index = self.states.findIndex(s => s.id === state.id);
    this.states.splice(index, 1);
  }

  public removeByNames(names: string[]): void {
    const filtered = this.states.filter((s) => {
      if (s.name) {
        return names.indexOf(s.name) > -1;
      }

      return false;
    });
    filtered.forEach((f) => {
      this.remove(f);
    });
  }

  public getState(stateName: string) {
    const filtered = this.states.filter((s) => s.id === stateName);
    return filtered.length === 0 ? null : filtered[0];
  }

  public getRootState(): StateMachineState | null {
    const filtered = this.states.filter((s) => this.start && this.start.stateName && s.id === this.start.stateName);
    return filtered.length === 0 ? null : filtered[0];
  }

  public getJson(): any {
    return {
      id: this.id,
      version: this.version,
      name: this.name,
      description: this.description,
      specVersion: this.specVersion,
      start: this.start,
      states: this.states.map((s: StateMachineState) => {
        return s.getJson();
      })
    };
  }

  public static build(json: any) : StateMachineModel {
    var result = new StateMachineModel();
    result.id = json["id"];
    result.name = json["name"];
    result.description = json["description"];
    result.version = json["version"];
    result.specVersion = json["specVersion"];
    if (json["start"]) {
      result.start = StartStateMachineModel.build(json['start']);
    }

    result.states = json["states"].map((s: any) => {
      switch (s.type) {
        case InjectStateMachineState.TYPE:
          {
            var result = new InjectStateMachineState();
            result.id = s["id"];
            if (s["data"]) {
              result.data = s["data"];
            }
            result.transition = s["transition"];
            result.name = s["name"];
            result.end = s["end"];
            result.stateDataFilter = StateDataFilter.build(s["stateDataFilter"]);
            return result;
          }
        case SwitchStateMachineState.TYPE:
          {
            var switchResult = new SwitchStateMachineState();
            switchResult.id = s["id"];
            switchResult.name = s["name"];
            switchResult.end = s["end"];
            switchResult.stateDataFilter = StateDataFilter.build(s["stateDataFilter"]);
            if (s["eventConditions"]) {
              switchResult.eventConditions = s["eventConditions"].map((ec: any) => {
                return EventCondition.build(ec);
              });
            }

            if (s["dataConditions"]) {
              switchResult.dataConditions = s["dataConditions"].map((dc: any) => {
                return DataCondition.build(dc);
              });
            }
            if (s["defaultCondition"]) {
              return switchResult.defaultCondition = EmptyTransition.build(s["defaultCondition"]);
            }

            return switchResult;
          }
          break;
      }

      return 
    });
    return result;
  }
}
