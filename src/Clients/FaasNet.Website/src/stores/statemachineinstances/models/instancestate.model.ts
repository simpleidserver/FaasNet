import { InstanceStateEvent } from "./instancestateevent.model";
import { InstanceStateHistory } from "./instancestatehistory.model";

export class InstanceState {
  constructor() {
    this.events = [];
    this.histories = [];
  }

  id: string = "";
  defId: string = "";
  status: string = "";
  input: any;
  output: any;
  events: InstanceStateEvent[] = [];
  histories: InstanceStateHistory[] = [];
}


export class InstanceStateStatus {
  static CREATE: string = "CREATE";
  static ACTIVE: string = "ACTIVE";
  static PENDING: string = "PENDING";
  static COMPLETE: string = "COMPLETE";
  static ERROR: string = "ERROR";
}
