import { ApplicationLink } from "./link.model";

export class Application {
  id: string = "";
  title: string = "";
  version: string = "";
  description: string = "";
  posX: number = 0;
  posY: number = 0;
  links: ApplicationLink[] = [];
}
