import { StateMachineState } from "./statemachine-state.model";

export class StateMachine {
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

  public remove(state: StateMachineState) : void {
    const index = this.states.indexOf(state);
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
    const filtered = this.states.filter((s) => s.name === stateName);
    return filtered.length === 0 ? null : filtered[0];
  }

  public getRootState() : StateMachineState | null {
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
}
