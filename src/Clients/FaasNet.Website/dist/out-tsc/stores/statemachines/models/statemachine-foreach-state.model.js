import { EmptyTransition, StateMachineState } from "./statemachine-state.model";
export class ForeachStateMachineState extends StateMachineState {
    constructor() {
        super();
        this.type = ForeachStateMachineState.TYPE;
        this.transition = "";
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
            transition: this.transition,
            type: ForeachStateMachineState.TYPE,
            end: this.end
        };
        if (this.stateDataFilter) {
            result['stateDataFilter'] = this.stateDataFilter.getJson();
        }
        return result;
    }
}
ForeachStateMachineState.TYPE = "foreach";
//# sourceMappingURL=statemachine-foreach-state.model.js.map