export class StateMachineFunction {
    getJson() {
        var result = {
            name: this.name,
            type: this.type,
            operation: this.operation
        };
        if (this.metadata) {
            result['metadata'] = this.metadata;
        }
        return result;
    }
    static build(json) {
        var result = new StateMachineFunction();
        result.name = json["name"];
        if (json["type"]) {
            result.type = json["type"].toLowerCase();
        }
        result.operation = json["operation"];
        result.metadata = json["metadata"];
        return result;
    }
}
//# sourceMappingURL=statemachine-function.model.js.map