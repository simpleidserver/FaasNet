import { Application } from "./application.model";
import { Message } from "./message";

export class ApplicationLink {
  evts: Message[] = [];
  target: Application = new Application();
}
