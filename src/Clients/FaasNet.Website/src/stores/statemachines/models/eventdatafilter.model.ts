export class EventDataFilter {
  useData: boolean = true;
  data: string | undefined;
  toStateData: string | undefined;

  public getJson(): any {
    var result: any = {
      useResults: this.useData
    };
    if (this.data) {
      result["data"] = this.data;
    }

    if (this.toStateData) {
      result["toStateData"] = this.toStateData;
    }

    return result;
  }

  public static build(json: any) {
    var result = new EventDataFilter();
    result.useData = json["useData"];
    if (json["data"]) {
      result.data = json["data"];
    }

    if (json["toStateData"]) {
      result.toStateData = json["toStateData"];
    }

    return result;
  }
}
