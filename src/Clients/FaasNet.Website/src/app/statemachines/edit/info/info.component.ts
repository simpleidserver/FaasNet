import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startUpdateInfo } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'display-state-machine-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.scss']
})
export class StateMachineInfoComponent implements OnInit {
  isLoading: boolean = false;
  private _stateMachineModel: StateMachineModel = new StateMachineModel();
  stateMachineFormGroup: FormGroup = new FormGroup({
    id: new FormControl({value: '', disabled: true}),
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    version: new FormControl({ value: '', disabled: true }),
    applicationDomainId: new FormControl(),
    vpn: new FormControl()
  });
  @Input()
  get stateMachine() {
    return this._stateMachineModel;
  }
  set stateMachine(value: StateMachineModel) {
    this._stateMachineModel = value;
    if (this._stateMachineModel) {
      this.init();
    }
  }

  get applicationDomainId() {
    return this.stateMachineFormGroup.get('applicationDomainId')?.value;
  }

  get vpn() {
    return this.stateMachineFormGroup.get('vpn')?.value;
  }

  constructor(
    private store: Store<fromReducers.AppState>,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private activatedRoute: ActivatedRoute) {

  }

  ngOnInit() {
    const self = this;
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] ERROR_UPDATE_STATEMACHINE_INFO'))
      .subscribe(() => {
        this.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.errorUpdateStateMachineInfo'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] COMPLETE_UPDATE_STATEMACHINE_INFO'))
      .subscribe(() => {
        this.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.updateStateMachineInfo'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.stateMachineFormGroup.reset();
      });
  }

  private init() {
    this.stateMachineFormGroup.get('id')?.setValue(this.activatedRoute.snapshot.params['id']);
    this.stateMachineFormGroup.get('name')?.setValue(this._stateMachineModel.name);
    this.stateMachineFormGroup.get('description')?.setValue(this._stateMachineModel.description);
    this.stateMachineFormGroup.get('version')?.setValue(this._stateMachineModel.version);
    this.stateMachineFormGroup.get('applicationDomainId')?.setValue(this._stateMachineModel.applicationDomainId);
    this.stateMachineFormGroup.get('vpn')?.setValue(this._stateMachineModel.vpn);
  }

  public update() {
    if (!this._stateMachineModel || !this._stateMachineModel.id || !this.stateMachineFormGroup.valid) {
      return;
    }

    this.isLoading = true;
    const action = startUpdateInfo({ id: this.activatedRoute.snapshot.params['id'], name: this.stateMachineFormGroup.get('name')?.value, description: this.stateMachineFormGroup.get('description')?.value });
    this.store.dispatch(action);
  }
}
