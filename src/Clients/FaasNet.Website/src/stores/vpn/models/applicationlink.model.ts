import { ApplicationResult } from "./application.model";
import { MessageDefinitionResult } from "./messagedefinition.model";

export class ApplicationLinkResult {
  topicName: string = "";
  message: MessageDefinitionResult | null = null;
  target: ApplicationResult | null = null;
}