import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startGetJson, startLaunch, startUpdate } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';
import { LaunchStateMachineComponent } from '../launch/launch-statemachine.component';

@Component({
  selector: 'edit-state-machine',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditStateMachineComponent implements OnInit, OnDestroy {
  stateMachineDef: StateMachineModel = new StateMachineModel();
  isLoading: boolean = false;
  id: string = "";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
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
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] COMPLETE_UPDATE_STATE_MACHINE'))
      .subscribe((evt : any) => {
        this.isLoading = false;
        this.router.navigate(['/statemachines/' + evt.id]);
        self.id = evt.id;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.stateMachineUpdated'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] ERROR_UPDATE_STATE_MACHINE'))
      .subscribe(() => {
        this.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.errorUpdateStateMachine'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] COMPLETE_LAUNCH_STATE_MACHINE'))
      .subscribe((e: any) => {
        console.log(e);
        this.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.stateMachineLaunched'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    self.actions$.pipe(
      filter((action: any) => action.type === '[StateMachines] ERROR_LAUNCH_STATE_MACHINE'))
      .subscribe(() => {
        this.isLoading = false;
        self.snackBar.open(this.translateService.instant('stateMachines.messages.errorLaunchStateMachine'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectStateMachineResult)).subscribe((stateMachine: any) => {
      if (!stateMachine) {
        return;
      }

      this.stateMachineDef = StateMachineModel.build(stateMachine);
    });
    self.id = this.activatedRoute.snapshot.params["id"];
    self.init();
  }

  ngOnDestroy() {
  }

  onSave(stateMachineModel: StateMachineModel) {
    this.isLoading = true;
    const command = startUpdate({ id : this.id, stateMachine: stateMachineModel.getJson() });
    this.store.dispatch(command);
  }

  onLaunch() {
    const dialogRef = this.dialog.open(LaunchStateMachineComponent, {
      width: '800px',
      data: {
        stateMachine: this.stateMachineDef
      }
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt || !opt.json || !this.stateMachineDef.id) {
        return;
      }

      this.isLoading = true;
      const action = startLaunch({ id: this.id, input: opt.json, parameters: opt.parameters });
      this.store.dispatch(action);
    });
  }

  private init(): void {
    const action = startGetJson({ id: this.id });
    this.store.dispatch(action);
  }
}
