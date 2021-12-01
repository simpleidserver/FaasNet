import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { InjectStateMachineState } from "../../models/statemachine-inject-state.model";
import { StateMachineState } from "../../models/statemachine-state.model";

@Component({
  selector: 'inject-state-editor',
  templateUrl: './inject-state-editor.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class InjectStateEditorComponent {
  private _state: StateMachineState | null = null;
  private _injectState : InjectStateMachineState | null = null;
  @Input()
  get state(): StateMachineState | null {
    return this._state;
  }
  set state(v : StateMachineState | null) {
    this._state = v;
    this._injectState = v as InjectStateMachineState;
    this.init();
  }

  updateInjectFormGroup: FormGroup = new FormGroup({
    data: new FormControl()
  });

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  saveForm() {
    const formVal = this.updateInjectFormGroup.value;
    const data = JSON.parse(formVal.data);
    if (data && this._injectState) {
      this._injectState.data = data;
    }
  }

  private init() {
    let json: string = "{}";
    if (this._injectState && this._injectState.data) {
      json = JSON.stringify(this._injectState.data);
    }

    this.updateInjectFormGroup.get('data')?.setValue(json);
  }
}
