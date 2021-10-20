export class ApiDefinitionResult {
  name: string | undefined;
  path: string | undefined;
  createDateTime: Date | undefined;
  updateDateTime: Date | undefined;
  operations: ApiDefinitionOperationResult[] = [];
}

export class ApiDefinitionOperationResult {
  name: string | undefined;
  path: string | undefined;
  createDateTime: Date | undefined;
  updateDateTime: Date | undefined;
  ui: ApiDefinitionOperationUIResult | undefined;
}

export class ApiDefinitionOperationUIResult {
  html: string | undefined;
  blocks: ApiDefinitionOperationBlockResult[] = [];
  blockarr: ApiDefinitionOperationBlockUIResult[] = [];
}

export class ApiDefinitionOperationBlockUIResult {
  childwidth: number | undefined;
  height: number | undefined;
  id: number | undefined;
  parent: number | undefined;
  width: number | undefined;
  x: number | undefined;
  y: number | undefined;
}

export class ApiDefinitionOperationBlockResult {
  id: string | undefined;
  parent: string | undefined;
  data: ApiDefinitionOperationBlockDataResult[] = [];
  model: ApiDefinitionOperationBlockModelResult | undefined;
}

export class ApiDefinitionOperationBlockDataResult {
  name: string | undefined;
  value: string = "";
}

export class ApiDefinitionOperationBlockModelResult {
  configuration: any | undefined;
  info: ApiDefinitionOperationBlockModelInfoResult | undefined;
}

export class ApiDefinitionOperationBlockModelInfoResult {
  name: string | undefined;
}
