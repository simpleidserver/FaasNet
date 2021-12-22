export class StateMachineFunction {
  name: string | undefined;
  type: string | undefined;
  operation: string | undefined;
  metadata: any;

  getJson(): any {
    return {
      name: this.name,
      type: this.type,
      operation: this.operation,
      metadata: this.metadata
    };
  }

  public static build(json: any): StateMachineFunction {
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
