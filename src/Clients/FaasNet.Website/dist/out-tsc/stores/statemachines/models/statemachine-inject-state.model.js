import { FlowableStateMachineState } from "./statemachine-state.model";
export class InjectStateMachineState extends FlowableStateMachineState {
    constructor() {
        super();
        this.data = null;
        this.type = InjectStateMachineState.TYPE;
    }
    getJson() {
        let result = {
            id: this.id,
            name: this.name,
            transition: this.transition,
            type: InjectStateMachineState.TYPE,
            end: this.end
        };
        if (this.stateDataFilter) {
            result['stateDataFilter'] = this.stateDataFilter.getJson();
        }
        if (this.data && Object.keys(this.data).length > 0) {
            result['data'] = this.data;
        }
        return result;
    }
}
InjectStateMachineState.TYPE = "inject";
//# sourceMappingURL=statemachine-inject-state.model.js.map