import { FunctionResult } from "@stores/functions/models/function.model";
import { BaseNodeModel } from "../base-node.model";

export const TYPE: string = "function";

export class FunctionModel extends BaseNodeModel<FunctionRecord>{
  constructor(content: FunctionRecord) {
    super();
    this.content = content;
    this.type = TYPE;
  }
}

export class FunctionRecord {
  info: FunctionResult | null = null;
  configuration: any;
}
