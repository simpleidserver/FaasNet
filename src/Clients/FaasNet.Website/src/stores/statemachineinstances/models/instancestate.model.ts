import { InstanceStateEvent } from "./instancestateevent.model";

export class InstanceState {
  constructor() {
    this.events = [];
  }

  id: string = "";
  defId: string = "";
  status: string = "";
  input: any;
  output: any;
  events: InstanceStateEvent[] = [];
}
