import { Component, Input } from "@angular/core";
import { StateMachineState } from "../../models/statemachine-state.model";

@Component({
  selector: 'switch-state-editor',
  templateUrl: './switch-state-editor.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class SwitchStateEditorComponent {
  @Input() state: StateMachineState | null = null;

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }
}
