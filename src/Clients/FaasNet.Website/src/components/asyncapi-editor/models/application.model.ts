import { ApplicationLink } from "./link.model";

export class Application {
  id: string = "";
  title: string | null = null;
  version: string | null = null;
  description: string | null = null;
  posX: number = 0;
  posY: number = 0;
  links: ApplicationLink[] = [];
}
