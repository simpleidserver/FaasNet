import { EmptyTransition, FlowableStateMachineState } from "./statemachine-state.model";
export class OperationStateMachineState extends FlowableStateMachineState {
    constructor() {
        super();
        this.actions = [];
        this.type = OperationStateMachineState.TYPE;
        this.actionMode = OperationStateMachineState.SEQUENTIAL;
    }
    setTransitions(transitions) {
        if (transitions.length > 0) {
            this.transition = transitions[0].transition;
            this.end = false;
        }
        else {
            this.end = true;
            this.transition = "";
        }
    }
    tryAddTransition(transitionName) {
        const oldTransition = this.transition;
        this.end = false;
        this.transition = transitionName;
        return oldTransition;
    }
    getNextTransitions() {
        const record = new EmptyTransition();
        record.transition = this.transition;
        return [
            record
        ];
    }
    getJson() {
        var _a;
        var result = {
            id: this.id,
            name: this.name,
            stateDataFilter: (_a = this.stateDataFilter) === null || _a === void 0 ? void 0 : _a.getJson(),
            type: OperationStateMachineState.TYPE,
            actionMode: this.actionMode,
            end: this.end,
            actions: this.actions.map((a) => a.getJSON())
        };
        if (this.transition) {
            result.transition = this.transition;
        }
        return result;
    }
}
OperationStateMachineState.TYPE = "operation";
OperationStateMachineState.SEQUENTIAL = "sequential";
OperationStateMachineState.PARALLEL = "parallel";
export class OperationAction {
    getJSON() {
        var result = {
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
    static build(json) {
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
    getJson() {
        let result = {
            refName: this.refName
        };
        if (this.arguments) {
            result["arguments"] = this.arguments;
        }
        return result;
    }
    static build(json) {
        var result = new OperationActionFunctionRef();
        if (json["arguments"]) {
            result.arguments = json["arguments"];
        }
        result.refName = json["refName"];
        return result;
    }
}
export class ActionDataFilter {
    constructor() {
        this.useResults = true;
    }
    getJson() {
        var result = {
            useResults: this.useResults
        };
        if (this.results) {
            result["results"] = this.results;
        }
        if (this.toStateData) {
            result["toStateData"] = this.toStateData;
        }
        return result;
    }
    static build(json) {
        var result = new ActionDataFilter();
        result.useResults = json["useResults"];
        if (json["results"]) {
            result.results = json["results"];
        }
        if (json["toStateData"]) {
            result.toStateData = json["toStateData"];
        }
        return result;
    }
}
//# sourceMappingURL=statemachine-operation-state.model.js.map