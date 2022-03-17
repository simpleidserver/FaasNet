import { BehaviorSubject } from "rxjs";
import { BaseTransition, EmptyTransition, StateMachineState } from "./statemachine-state.model";
export class EventCondition extends BaseTransition {
    constructor() {
        super();
        this._eventRef = "";
        this.label = new BehaviorSubject("");
    }
    get eventRef() {
        return this._eventRef;
    }
    set eventRef(str) {
        var _a;
        this._eventRef = str;
        (_a = this.label) === null || _a === void 0 ? void 0 : _a.next(this._eventRef);
    }
    getType() {
        return EventCondition.TYPE;
    }
    getLabel() {
        return new BehaviorSubject("event");
    }
    getJson() {
        return {
            eventRef: this.eventRef,
            transition: this.transition
        };
    }
    static build(json) {
        var result = new EventCondition();
        result.eventRef = json["eventRef"];
        result.label = json["label"];
        result.transition = json["transition"];
        return result;
    }
}
EventCondition.TYPE = "event";
export class DataCondition extends BaseTransition {
    constructor() {
        super();
        this._condition = "";
        this.label = new BehaviorSubject("");
    }
    get condition() {
        return this._condition;
    }
    set condition(str) {
        var _a;
        this._condition = str;
        (_a = this.label) === null || _a === void 0 ? void 0 : _a.next(this._condition);
    }
    getType() {
        return DataCondition.TYPE;
    }
    getLabel() {
        return this.label;
    }
    getJson() {
        var result = {
            name: this.name,
            transition: this.transition
        };
        if (this.condition) {
            result["condition"] = this.condition;
        }
        return result;
    }
    static build(json) {
        var result = new DataCondition();
        if (json["condition"]) {
            result.condition = json["condition"];
        }
        result.condition = json["condition"];
        result.name = json["name"];
        result.transition = json["transition"];
        return result;
    }
}
DataCondition.TYPE = "data";
export class SwitchStateMachineState extends StateMachineState {
    constructor() {
        super();
        this.switchType = "event";
        this.dataConditions = [];
        this.defaultCondition = null;
        this.type = SwitchStateMachineState.TYPE;
        this.eventConditions = [];
        this.dataConditions = [];
    }
    setTransitions(transitions) {
        if (transitions) {
            switch (this.switchType) {
                case SwitchStateMachineState.EVENT_TYPE:
                    this.eventConditions = transitions.filter((t) => {
                        return t.getType() == EventCondition.TYPE;
                    }).map((t) => {
                        return t;
                    });
                    break;
                case SwitchStateMachineState.DATA_TYPE:
                    this.dataConditions = transitions.filter((t) => {
                        return t.getType() == DataCondition.TYPE;
                    }).map((t) => {
                        return t;
                    });
                    break;
            }
        }
        this.end = transitions.length === 0;
        return [];
    }
    tryAddTransition(transitionName) {
        this.end = false;
        switch (this.switchType) {
            case SwitchStateMachineState.EVENT_TYPE:
                const newEvtTransition = new EventCondition();
                newEvtTransition.eventRef = "event";
                newEvtTransition.transition = transitionName;
                this.eventConditions.push(newEvtTransition);
                break;
            case SwitchStateMachineState.DATA_TYPE:
                const newDataCondition = new DataCondition();
                newDataCondition.condition = "true";
                newDataCondition.transition = transitionName;
                this.dataConditions.push(newDataCondition);
                break;
        }
        return null;
    }
    getNextTransitions() {
        let arr = [];
        this.eventConditions.forEach((ec) => {
            arr.push(ec);
        });
        this.dataConditions.forEach((dc) => {
            arr.push(dc);
        });
        if (this.defaultCondition && this.defaultCondition.transition) {
            arr.push(this.defaultCondition);
        }
        return arr;
    }
    switchToEventCondition() {
        this.switchType = SwitchStateMachineState.EVENT_TYPE;
        this.eventConditions = this.dataConditions.map((e) => {
            const record = new EventCondition();
            record.eventRef = "event";
            record.transition = e.transition;
            return record;
        });
        this.dataConditions = [];
    }
    switchToDataCondition() {
        this.switchType = SwitchStateMachineState.DATA_TYPE;
        this.dataConditions = this.eventConditions.map((e) => {
            const record = new DataCondition();
            record.transition = e.transition;
            record.condition = "true";
            return record;
        });
        this.eventConditions = [];
    }
    swichTransitionToDefault(transition) {
        if (this.defaultCondition) {
            if (transition.transition == this.defaultCondition.transition) {
                return;
            }
            this.tryAddTransition(this.defaultCondition.transition);
        }
        this.removeTransition(transition);
        this.defaultCondition = new EmptyTransition();
        this.defaultCondition.transition = transition.transition;
    }
    getJson() {
        var result = {
            id: this.id,
            name: this.name,
            type: SwitchStateMachineState.TYPE,
            end: this.end,
            dataConditions: this.dataConditions.map((d) => {
                return d.getJson();
            })
        };
        if (this.defaultCondition && this.defaultCondition.transition) {
            result.defaultCondition = {
                transition: this.defaultCondition.transition
            };
        }
        if (this.stateDataFilter) {
            result.stateDataFilter = this.stateDataFilter.getJson();
        }
        if (this.eventConditions && this.eventConditions.length > 0) {
            result.eventConditions = this.eventConditions.map((e) => {
                return e.getJson();
            });
        }
        if (this.dataConditions && this.dataConditions.length > 0) {
            result.dataConditions = this.dataConditions.map((d) => {
                return d.getJson();
            });
        }
        return result;
    }
    removeTransition(transition) {
        switch (this.switchType) {
            case SwitchStateMachineState.EVENT_TYPE:
                {
                    const index = this.eventConditions.indexOf(transition);
                    this.eventConditions.splice(index, 1);
                }
                break;
            case SwitchStateMachineState.DATA_TYPE:
                {
                    const index = this.dataConditions.indexOf(transition);
                    this.dataConditions.splice(index, 1);
                }
                break;
        }
    }
}
SwitchStateMachineState.TYPE = "switch";
SwitchStateMachineState.EVENT_TYPE = "event";
SwitchStateMachineState.DATA_TYPE = "data";
export class DefaultCondition {
}
//# sourceMappingURL=statemachine-switch-state.model.js.map