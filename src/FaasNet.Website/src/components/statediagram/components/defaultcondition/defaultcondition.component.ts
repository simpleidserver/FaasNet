import { Component, EventEmitter, Output } from "@angular/core";

@Component({
  selector: 'defaultcondition-editor',
  templateUrl: './defaultcondition.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class DefaultConditionComponent {
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();
  close() {
    this.closed.emit();
  }
}
