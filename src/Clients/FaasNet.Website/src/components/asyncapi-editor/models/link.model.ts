import { MessageDefinitionResult } from "@stores/vpn/models/messagedefinition.model";
import { Application } from "./application.model";

export class ApplicationLink {
  message: MessageDefinitionResult | null = null;
  target: Application = new Application();
  topicName: string = "";
}
