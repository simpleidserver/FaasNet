import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startGet } from '@stores/statemachineinstances/actions/statemachineinstances.actions';
import { StateMachineInstanceDetails } from '@stores/statemachineinstances/models/statemachineinstance-details.model';
import { startGetJson } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-state-machineinstance',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewStateMachineInstanceComponent implements OnInit, OnDestroy {
  stateMachineDef: StateMachineModel = new StateMachineModel();
  stateMachineInstance: StateMachineInstanceDetails = new StateMachineInstanceDetails();
  isLoading: boolean = false;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private router: Router) {
  }

  ngOnInit() {
    const self = this;
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] ERROR_GET_JSON_STATE_MACHINE'))
      .subscribe(() => {
        self.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.errorGetStateMachine'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachineInstances] ERROR_GET_STATEMACHINE_INSTANCE'))
      .subscribe(() => {
        self.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachineInstance.messages.errorGetStateMachineInstance'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectStateMachineInstanceResult)).subscribe((stateMachineInstance: any) => {
      if (!stateMachineInstance) {
        return;
      }

      self.stateMachineInstance = stateMachineInstance;
      self.refreshLoading();
    });
    this.store.pipe(select(fromReducers.selectStateMachineResult)).subscribe((stateMachine: any) => {
      if (!stateMachine) {
        return;
      }

      this.stateMachineDef = StateMachineModel.build(stateMachine);
      self.refreshLoading();
    });
    self.init();
  }

  ngOnDestroy() {
  }

  private refreshLoading() {
    if (this.stateMachineDef && this.stateMachineInstance && this.stateMachineDef.id && this.stateMachineInstance.id) {
      this.isLoading = false;
    }
  }

  private init(): void {
    this.isLoading = true;
    const id = this.activatedRoute.snapshot.params['id'];
    const instid = this.activatedRoute.snapshot.params['instid'];
    const startGetDef = startGetJson({ id: id });
    const startGetInstance = startGet({ id: instid });
    this.store.dispatch(startGetDef);
    this.store.dispatch(startGetInstance);
  }
}
