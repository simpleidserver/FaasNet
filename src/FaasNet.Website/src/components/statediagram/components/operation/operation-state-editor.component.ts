import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { OperationStateMachineState } from "@stores/statemachines/models/statemachine-operation-state.model";
import { StateMachineState } from "@stores/statemachines/models/statemachine-state.model";
import { StateMachineModel } from "@stores/statemachines/models/statemachinemodel.model";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ActionsEditorComponent, ActionsEditorData } from "./actionseditor.component";

@Component({
  selector: 'operation-state-editor',
  templateUrl: './operation-state-editor.component.html',
  styleUrls: [
    './operation-state-editor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class OperationStateEditorComponent implements OnDestroy {
  private nameSubscription: any = null;
  private actionModeSubscription: any = null;
  private _state: StateMachineState | null = null;
  private _operationState: OperationStateMachineState | null = null;
  @Input() stateMachine: StateMachineModel | null = null;
  @Input()
  get state(): StateMachineState | null {
    return this._state;
  }
  set state(v: StateMachineState | null) {
    this._state = v;
    this._operationState = v as OperationStateMachineState;
    this.init();
  }
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();
  get end() {
    return this._operationState?.end;
  }

  updateOperationFormGroup: FormGroup = new FormGroup({
    name: new FormControl(),
    end: new FormControl(),
    actionMode: new FormControl()
  });

  constructor(private matPanelService: MatPanelService) { }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.nameSubscription) {
      this.nameSubscription.unsubscribe();
    }

    if (this.actionModeSubscription) {
      this.actionModeSubscription.unsubscribe();
    }
  }

  close() {
    this.closed.emit();
  }

  getActions() {
    return this._operationState?.actions.map((a) => a.name).join(',');
  }

  editActions() {
    var data = new ActionsEditorData();
    if (this.stateMachine) {
      data.functions = this.stateMachine.functions;
    }

    if (this._operationState) {
      data.actions = this._operationState.actions;
    }

    this.matPanelService.open(ActionsEditorComponent, data);
  }

  private init() {
    const self = this;
    if (self._operationState) {
      self.updateOperationFormGroup.get('name')?.setValue(self._operationState.name);
      self.updateOperationFormGroup.get('actionMode')?.setValue(self._operationState.actionMode);
    }

    self.updateOperationFormGroup.get('end')?.setValue(self._operationState?.end);
    self.updateOperationFormGroup.get('end')?.disable();

    this.nameSubscription = this.updateOperationFormGroup.get('name')?.valueChanges.subscribe((e: any) => {
      if (self.updateOperationFormGroup && self._operationState) {
        self._operationState.name = e;
      }
    });

    this.actionModeSubscription = this.updateOperationFormGroup.get('actionMode')?.valueChanges.subscribe((e: any) => {
      if (self.updateOperationFormGroup && self._operationState) {
        self._operationState.actionMode = e;
      }
    });
  }
}
