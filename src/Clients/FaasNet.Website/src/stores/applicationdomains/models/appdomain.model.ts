import { ApplicationResult } from "./application.model";

export class AppDomainResult {
  id: string = "";
  name: string = "";
  description: string = "";
  rootTopic: string = "";
  createDateTime: Date | null = null;
  updateDateTime: Date | null = null;
  applications: ApplicationResult[] = [];
}
