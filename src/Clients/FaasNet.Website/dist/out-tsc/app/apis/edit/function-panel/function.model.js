import { BaseNodeModel } from "../base-node.model";
export const TYPE = "function";
export class FunctionModel extends BaseNodeModel {
    constructor(content) {
        super();
        this.content = content;
        this.type = TYPE;
    }
}
export class FunctionRecord {
    constructor() {
        this.info = null;
    }
}
//# sourceMappingURL=function.model.js.map