import { Component, Input, OnDestroy } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { StateMachineState } from "../../models/statemachine-state.model";
import { SwitchStateMachineState } from "../../models/statemachine-switch-state.model";

@Component({
  selector: 'switch-state-editor',
  templateUrl: './switch-state-editor.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class SwitchStateEditorComponent implements OnDestroy {
  private nameSubscription: any = null;
  private typeSubscription: any = null;
  private _state: StateMachineState | null = null;
  private _switchState: SwitchStateMachineState | null = null;
  @Input()
  get state(): StateMachineState | null {
    return this._state;
  }
  set state(v: StateMachineState | null) {
    this._state = v;
    this._switchState = v as SwitchStateMachineState;
    this.init();
  }
  updateSwitchFormGroup: FormGroup = new FormGroup({
    name: new FormControl(),
    type: new FormControl()
  });

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.nameSubscription) {
      this.nameSubscription.unsubscribe();
    }

    if (this.typeSubscription) {
      this.typeSubscription.unsubscribe();
    }
  }

  private init() {
    const self = this;
    if (self._switchState) {
      self.updateSwitchFormGroup.get('name')?.setValue(self._switchState.name);
      if (this._switchState?.dataConditions && this._switchState.dataConditions.length > 0) {
        self.updateSwitchFormGroup.get('type')?.setValue('data');
      } else {
        self.updateSwitchFormGroup.get('type')?.setValue(self._switchState.switchType);
      }
    }

    this.nameSubscription = this.updateSwitchFormGroup.get('name')?.valueChanges.subscribe((e: any) => {
      if (self.updateSwitchFormGroup && self._switchState) {
        self._switchState.name = e;
      }
    });
    this.typeSubscription = this.updateSwitchFormGroup.get('type')?.valueChanges.subscribe((e: any) => {
      switch (e) {
        case SwitchStateMachineState.DATA_TYPE:
          self._switchState?.switchToDataCondition();
          break;
        case SwitchStateMachineState.EVENT_TYPE:
          self._switchState?.switchToEventCondition();
          break;
      }

      if (self._switchState) {
        this._switchState?.updated.emit(self._switchState);
      }
    });
  }
}
