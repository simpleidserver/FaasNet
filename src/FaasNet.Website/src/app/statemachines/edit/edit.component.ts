import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startGetJson } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'edit-state-machine',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditStateMachineComponent implements OnInit, OnDestroy {
  stateMachineDef: StateMachineModel = new StateMachineModel();

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) {
  }

  ngOnInit() {
    const self = this;
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] ERROR_GET_JSON_STATE_MACHINE'))
      .subscribe(() => {
        self.snackBar.open(this.translateService.instant('stateMachines.messages.errorGetStateMachine'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectStateMachineResult)).subscribe((stateMachine: StateMachineModel | null) => {
      if (!stateMachine) {
        return;
      }

      this.stateMachineDef = stateMachine;
    });
    self.init();
  }

  ngOnDestroy() {
  }

  private init(): void {
    const id = this.activatedRoute.snapshot.params['id'];
    const action = startGetJson({ id: id });
    this.store.dispatch(action);
  }
}
