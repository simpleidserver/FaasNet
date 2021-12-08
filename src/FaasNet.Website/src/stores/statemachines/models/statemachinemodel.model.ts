import { InjectStateMachineState } from "./statemachine-inject-state.model";
import { BaseTransition, StateDataFilter, StateMachineState } from "./statemachine-state.model";

export class StateMachineModel {
  constructor() {
    this.states = [];
  }

  id: string | undefined;
  version: string | undefined;
  specVersion: string | undefined;
  start: string | undefined;
  states: StateMachineState[];

  get isEmpty() {
    return this.states.length === 0;
  }

  public remove(state: StateMachineState): void {
    const self = this;
    const index = self.states.indexOf(state);
    const nextTransitions = state.getNextTransitions();
    nextTransitions.forEach((t: BaseTransition) => {
      const child = self.getState(t.transition);
      if (child) {
        self.remove(child);
      }
    });

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
    let rootState: StateMachineState | null = null;
    this.states.forEach((s) => {
      const filtered = this.states.filter((c) => {
        if (!c.name || !s.name) {
          return false;
        }

        return c.getNextTransitions().map(t => t.transition).indexOf(s.name) > -1;
      });
      if (filtered.length === 0 && !rootState) {
        rootState = s;
      }
    });

    return rootState;
  }

  public getJson(): any {
    return {
      id: this.id,
      version: this.version,
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
    result.version = json["version"];
    result.specVersion = json["specVersion"];
    result.start = json["start"];
    result.states = json["states"].map((s: any) => {
      switch (s.type) {
        case InjectStateMachineState.TYPE:
          var result = new InjectStateMachineState();
          result.data = s["data"];
          result.transition = s["transition"];
          result.name = s["name"];
          result.stateDataFilter = StateDataFilter.build(s["stateDataFilter"]);
          return result;
      }

      return 
    });
    return result;
  }
}
