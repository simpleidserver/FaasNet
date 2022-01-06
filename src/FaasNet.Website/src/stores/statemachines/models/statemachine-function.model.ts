export class StateMachineFunction {
  name: string | undefined;
  type: string | undefined;
  operation: string | undefined;
  metadata: any;

  getJson(): any {
    var result : any = {
      name: this.name,
      type: this.type,
      operation: this.operation
    };
    if (this.metadata) {
      result['metadata'] = this.metadata;
    }

    return result;
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
