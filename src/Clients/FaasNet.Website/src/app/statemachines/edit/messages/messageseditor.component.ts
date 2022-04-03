import { Component, Input } from '@angular/core';

@Component({
  selector: 'messageseditor-statemachine',
  templateUrl: './messageseditor.component.html'
})
export class StateMachineMessagesEditorComponent {
  @Input() appDomainId: string | undefined = "";

  constructor() {
  }
}
