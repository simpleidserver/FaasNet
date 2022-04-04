export class StateMachineEvent {
  name: string = "";
  type: string = "";
  source: string = "";
  kind: string = "";


  public static build(json: any): StateMachineEvent {
    var result = new StateMachineEvent();
    result.name = json["name"];
    result.type = json["type"];
    result.source = json["source"];
    result.kind = json["kind"].toLowerCase();
    return result;
  }

  getJson() {
    return {
      name: this.name,
      type: this.type,
      source: this.source,
      kind: this.kind
    };
  }
}
