import { BaseTransition, EmptyTransition, FlowableStateMachineState } from "./statemachine-state.model";

export class OperationStateMachineState extends FlowableStateMachineState {
  public static TYPE: string = "operation";
  public static SEQUENTIAL = "sequential";
  public static PARALLEL = "parallel";

  constructor() {
    super();
    this.type = OperationStateMachineState.TYPE;
    this.actionMode = OperationStateMachineState.SEQUENTIAL;
  }

  actionMode: string | undefined;
  actions: OperationAction[] = [];

  public override setTransitions(transitions: BaseTransition[]) {
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
    var result : any = {
      id: this.id,
      name: this.name,
      stateDataFilter: this.stateDataFilter?.getJson(),
      type: OperationStateMachineState.TYPE,
      actionMode: this.actionMode,
      end: this.end,
      actions: this.actions.map((a) => a.getJSON())
    };
    return result;
  }
}

export class OperationAction {
  name: string | undefined;
  functionRef: OperationActionFunctionRef | undefined;
  actionDataFilter: ActionDataFilter | undefined;

  public getJSON() {
    var result: any = {
      name: this.name
    };
    if (this.functionRef) {
      result['functionRef'] = this.functionRef.getJson();
    }

    if (this.actionDataFilter) {
      result["actionDataFilter"] = this.actionDataFilter.getJson();
    }

    return result;
  }

  public static build(json: any) {
    var result = new OperationAction();
    result.name = json["name"];
    if (json["functionRef"]) {
      result.functionRef = OperationActionFunctionRef.build(json["functionRef"]);
    }

    if (json["actionDataFilter"]) {
      result.actionDataFilter = ActionDataFilter.build(json["actionDataFilter"]);
    }

    return result;
  }
}

export class OperationActionFunctionRef {
  refName: string | undefined;
  arguments: any;

  public getJson(): any {
    return {
      refName: this.refName,
      arguments: this.arguments
    };
  }

  public static build(json: any) {
    var result = new OperationActionFunctionRef();
    result.arguments = json["arguments"];
    result.refName = json["refName"];
    return result;
  }
}

export class ActionDataFilter {
  useResults: boolean = true;
  results: string | undefined;
  toStateData: string | undefined;

  public getJson(): any {
    return {
      useResults: this.useResults,
      results: this.results,
      toStateData: this.toStateData
    }
  }

  public static build(json: any) {
    var result = new ActionDataFilter();
    result.useResults = json["useResults"];
    result.results = json["results"];
    result.toStateData = json["toStateData"];
    return result;
  }
}
