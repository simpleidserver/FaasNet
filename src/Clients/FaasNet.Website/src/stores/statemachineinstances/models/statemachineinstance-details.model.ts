import { InstanceState } from "./instancestate.model";

export class StateMachineInstanceDetails {
  id: string = "";
  workflowDefId: string = "";
  workflowDefName: string = "";
  workflowDefDescription: string = "";
  status: string = "";
  createDateTime: Date | undefined;
  output: any;
  states: InstanceState[] = [];
}
