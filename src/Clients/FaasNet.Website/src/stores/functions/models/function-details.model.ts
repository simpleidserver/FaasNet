export class FunctionDetailsResult {
  pods: FunctionPodResult[] = [];
}

export class FunctionPodResult {
  name: string| undefined;
  status: string | undefined;
  startTime: Date | undefined;
}
