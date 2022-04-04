import { ApplicationLinkResult } from "./applicationlink.model";

export class ApplicationResult {
  id: string = "";
  clientId: string | null = null;
  title: string = "";
  description: string = "";
  version: number = 0;
  isRoot: boolean = false;
  posX: number = 0;
  posY: number = 0;
  links: ApplicationLinkResult[] = [];
}
