import { EventEmitter } from "@angular/core";
import { BehaviorSubject } from "rxjs";
export class StateMachineState {
    constructor() {
        this.end = false;
        this.updated = new EventEmitter();
    }
}
export class StateDataFilter {
    constructor() {
        this.input = "";
        this.output = "";
    }
    getJson() {
        var result = {};
        if (this.input) {
            result["input"] = this.input;
        }
        if (this.output) {
            result["output"] = this.output;
        }
        return result;
    }
    static build(json) {
        if (!json) {
            return undefined;
        }
        var result = new StateDataFilter();
        if (json["input"]) {
            result.input = json["input"];
        }
        if (json["output"]) {
            result.output = json["output"];
        }
        return result;
    }
}
export class BaseTransition {
    constructor() {
        this.transition = "";
    }
}
export class EmptyTransition extends BaseTransition {
    constructor() {
        super();
        this.label = new BehaviorSubject("transition");
    }
    getType() {
        return EmptyTransition.TYPE;
    }
    getLabel() {
        return this.label;
    }
    static build(json) {
        var result = new EmptyTransition();
        result.label = json["label"];
        result.transition = json["transition"];
        return result;
    }
}
EmptyTransition.TYPE = "empty";
export class FlowableStateMachineState extends StateMachineState {
    constructor() {
        super();
        this.transition = "";
    }
    setTransitions(transitions) {
        if (transitions.length > 0) {
            this.transition = transitions[0].transition;
            this.end = false;
        }
        else {
            this.transition = "";
            this.end = true;
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
}
//# sourceMappingURL=statemachine-state.model.js.map