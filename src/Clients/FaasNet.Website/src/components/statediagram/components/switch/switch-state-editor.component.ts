import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { StateMachineState } from "@stores/statemachines/models/statemachine-state.model";
import { SwitchStateMachineState } from "@stores/statemachines/models/statemachine-switch-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";

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
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  get end() {
    return this._switchState?.end;
  }

  updateSwitchFormGroup: FormGroup = new FormGroup({
    name: new FormControl(),
    type: new FormControl(),
    inputStateDataFilter : new FormControl(),
    outputStateDataFilter: new FormControl(),
    end: new FormControl()
  });

  constructor(private dialog: MatDialog) {
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

  close() {
    this.closed.emit();
  }

  editInputStateDataFilter() {
    let filter: string = "";
    if (this._switchState?.stateDataFilter?.input) {
      filter = this._switchState?.stateDataFilter.input;
    }

    const dialogRef = this.dialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      if (this._switchState?.stateDataFilter) {
        this._switchState.stateDataFilter.input = r.filter;
      }

      this.updateSwitchFormGroup.get('inputStateDataFilter')?.setValue(r.filter);
    });
  }

  editOutputStateDataFilter() {
    let filter: string = "";
    if (this._switchState?.stateDataFilter?.output) {
      filter = this._switchState?.stateDataFilter.output;
    }

    const dialogRef = this.dialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      if (this._switchState?.stateDataFilter) {
        this._switchState.stateDataFilter.output = r.filter;
      }

      this.updateSwitchFormGroup.get('outputStateDataFilter')?.setValue(r.filter);
    });
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

    self.updateSwitchFormGroup.get('end')?.setValue(self._switchState?.end);
    self.updateSwitchFormGroup.get('end')?.disable();

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
