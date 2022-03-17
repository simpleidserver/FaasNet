import { ForeachStateMachineState } from "./statemachine-foreach-state.model";
import { StateMachineFunction } from "./statemachine-function.model";
import { InjectStateMachineState } from "./statemachine-inject-state.model";
import { OperationAction, OperationStateMachineState } from "./statemachine-operation-state.model";
import { EmptyTransition, StateDataFilter } from "./statemachine-state.model";
import { DataCondition, EventCondition, SwitchStateMachineState } from "./statemachine-switch-state.model";
export class StartStateMachineModel {
    getJson() {
        var result = {};
        if (this.stateName) {
            result['stateName'] = this.stateName;
        }
        if (this.schedule) {
            result['schedule'] = this.schedule;
        }
        return result;
    }
    static build(json) {
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
        this.functions = [];
    }
    get isEmpty() {
        return this.states.length === 0;
    }
    remove(state) {
        const self = this;
        const nextTransitions = state.getNextTransitions();
        nextTransitions.forEach((t) => {
            const child = self.getState(t.transition);
            if (child) {
                self.remove(child);
            }
        });
        const index = self.states.findIndex(s => s.id === state.id);
        this.states.splice(index, 1);
    }
    removeByNames(names) {
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
    getState(stateName) {
        const filtered = this.states.filter((s) => s.id === stateName);
        return filtered.length === 0 ? null : filtered[0];
    }
    getRootState() {
        const filtered = this.states.filter((s) => this.start && this.start.stateName && s.id === this.start.stateName);
        return filtered.length === 0 ? null : filtered[0];
    }
    getJson() {
        return {
            id: this.id,
            version: this.version,
            name: this.name,
            description: this.description,
            specVersion: this.specVersion,
            start: this.start,
            states: this.states.map((s) => {
                return s.getJson();
            }),
            functions: this.functions.map((s) => {
                return s.getJson();
            })
        };
    }
    static build(json) {
        var result = new StateMachineModel();
        result.id = json["id"];
        result.name = json["name"];
        result.description = json["description"];
        result.version = json["version"];
        result.specVersion = json["specVersion"];
        if (json["start"]) {
            result.start = StartStateMachineModel.build(json['start']);
        }
        result.states = json["states"].map((s) => {
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
                            switchResult.eventConditions = s["eventConditions"].map((ec) => {
                                return EventCondition.build(ec);
                            });
                        }
                        if (s["dataConditions"]) {
                            switchResult.dataConditions = s["dataConditions"].map((dc) => {
                                return DataCondition.build(dc);
                            });
                        }
                        if (s["defaultCondition"]) {
                            switchResult.defaultCondition = EmptyTransition.build(s["defaultCondition"]);
                        }
                        return switchResult;
                    }
                    break;
                case OperationStateMachineState.TYPE:
                    {
                        var operationResult = new OperationStateMachineState();
                        operationResult.id = s["id"];
                        operationResult.name = s["name"];
                        operationResult.end = s["end"];
                        operationResult.transition = s["transition"];
                        operationResult.actionMode = s["actionMode"];
                        operationResult.stateDataFilter = StateDataFilter.build(s["stateDataFilter"]);
                        if (s["actions"]) {
                            operationResult.actions = s["actions"].map((ac) => {
                                return OperationAction.build(ac);
                            });
                        }
                        return operationResult;
                    }
                    break;
                case ForeachStateMachineState.TYPE:
                    {
                        var foreachResult = new ForeachStateMachineState();
                        foreachResult.id = s["id"];
                        foreachResult.name = s["name"];
                        foreachResult.end = s["end"];
                        foreachResult.transition = s["transition"];
                        return foreachResult;
                    }
                    break;
            }
            return;
        });
        if (json["functions"]) {
            result.functions = json["functions"].map((f) => StateMachineFunction.build(f));
        }
        return result;
    }
}
//# sourceMappingURL=statemachinemodel.model.js.map