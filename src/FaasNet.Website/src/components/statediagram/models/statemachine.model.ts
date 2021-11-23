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
}
