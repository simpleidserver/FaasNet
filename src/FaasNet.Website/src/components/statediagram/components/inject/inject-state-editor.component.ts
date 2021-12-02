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
  private _injectState: InjectStateMachineState | null = null;
  private nameSubscription: any | null = null;
  private dataSubscription: any | null = null;
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
    name: new FormControl(),
    data: new FormControl()
  });

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.nameSubscription) {
      this.nameSubscription.unsubscribe();
    }

    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  private init() {
    const self = this;
    this.ngOnDestroy();
    let json: string = "{}";
    if (this._injectState && this._injectState.data) {
      json = JSON.stringify(this._injectState.data);
    }

    this.updateInjectFormGroup.get('data')?.setValue(json);
    this.updateInjectFormGroup.get('name')?.setValue(this._injectState?.name);
    this.nameSubscription = this.updateInjectFormGroup.get('name')?.valueChanges.subscribe((e: any) => {
      if (self._injectState) {
        self._injectState.name = e;
      }
    });
    this.dataSubscription = this.updateInjectFormGroup.get('data')?.valueChanges.subscribe((e : any) => {
      if (self._injectState) {
        if (e) {
          try {
            const data = JSON.parse(e);
            self._injectState.data = data;
          }
          catch { }
        }
      }
    });
  }
}
