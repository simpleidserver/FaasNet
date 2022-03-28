import { MessageDefinitionResult } from "../../messagedefinitions/models/messagedefinition.model";
import { ApplicationResult } from "./application.model";

export class ApplicationLinkResult {
  topicName: string = "";
  messageId: string | null = null;
  target: ApplicationResult | null = null;
  startAnchor: number = 0;
  endAnchor: number = 0;
}
