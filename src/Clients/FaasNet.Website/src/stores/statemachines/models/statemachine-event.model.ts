export class StateMachineEvent {
  name: string = "";
  type: string = "";
  source: string = "";
  kind: string = "";
  metadata: any;

  public static build(json: any): StateMachineEvent {
    var result = new StateMachineEvent();
    result.name = json["name"];
    result.type = json["type"];
    result.source = json["source"];
    result.kind = json["kind"].toLowerCase();
    if (json["metadata"]) {
      result.metadata = json["metadata"];
    }

    console.log(result);
    return result;
  }

  getJson() {
    let result : any = {
      name: this.name,
      type: this.type,
      source: this.source,
      kind: this.kind
    };
    console.log(this.metadata);
    if (this.metadata) {
      result.metadata = this.metadata;
    }

    console.log(result);
    return result;
  }
}
