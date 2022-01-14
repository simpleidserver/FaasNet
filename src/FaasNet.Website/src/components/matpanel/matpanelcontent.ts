import { EventEmitter } from "@angular/core";

export abstract class MatPanelContent {
  abstract init(evt: any): void;
  onClosed: EventEmitter<any> = new EventEmitter();
}
