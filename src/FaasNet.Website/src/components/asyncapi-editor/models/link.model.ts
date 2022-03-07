import { Application } from "./application.model";
import { Message } from "./message";

export class ApplicationLink {
  evt: Message = new Message();
  target: Application = new Application();
}
