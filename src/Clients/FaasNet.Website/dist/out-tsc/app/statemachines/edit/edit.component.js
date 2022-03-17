import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetJson, startLaunch, startUpdate } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';
import { LaunchStateMachineComponent } from './launch/launch-statemachine.component';
let EditStateMachineComponent = class EditStateMachineComponent {
    constructor(store, activatedRoute, router, actions$, translateService, snackBar, dialog) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.router = router;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.dialog = dialog;
        this.stateMachineDef = new StateMachineModel();
        this.isLoading = false;
        this.id = "";
        this.action = "";
    }
    ngOnInit() {
        const self = this;
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] ERROR_GET_JSON_STATE_MACHINE'))
            .subscribe(() => {
            self.snackBar.open(this.translateService.instant('stateMachines.messages.errorGetStateMachine'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] COMPLETE_UPDATE_STATE_MACHINE'))
            .subscribe((evt) => {
            this.isLoading = false;
            this.router.navigate(['/statemachines/' + evt.id + '/' + this.action]);
            self.id = evt.id;
            self.snackBar.open(this.translateService.instant('stateMachines.messages.stateMachineUpdated'), this.translateService.instant('undo'), {
                duration: 2000
            });
            self.init();
        });
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] ERROR_UPDATE_STATE_MACHINE'))
            .subscribe(() => {
            this.isLoading = false;
            self.snackBar.open(this.translateService.instant('stateMachines.messages.errorUpdateStateMachine'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] COMPLETE_LAUNCH_STATE_MACHINE'))
            .subscribe((e) => {
            console.log(e);
            this.isLoading = false;
            self.snackBar.open(this.translateService.instant('stateMachines.messages.stateMachineLaunched'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] ERROR_LAUNCH_STATE_MACHINE'))
            .subscribe(() => {
            this.isLoading = false;
            self.snackBar.open(this.translateService.instant('stateMachines.messages.errorLaunchStateMachine'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectStateMachineResult)).subscribe((stateMachine) => {
            if (!stateMachine) {
                return;
            }
            this.stateMachineDef = StateMachineModel.build(stateMachine);
        });
        self.id = self.activatedRoute.snapshot.params["id"];
        self.action = self.activatedRoute.snapshot.params['action'];
        self.routeSubscription = self.activatedRoute.params.subscribe(() => {
            self.action = self.activatedRoute.snapshot.params['action'];
        });
        self.init();
    }
    ngOnDestroy() {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }
    update() {
        this.isLoading = true;
        const command = startUpdate({ id: this.id, stateMachine: this.stateMachineDef.getJson() });
        this.store.dispatch(command);
    }
    launch() {
        const dialogRef = this.dialog.open(LaunchStateMachineComponent, {
            width: '800px',
            data: {
                stateMachine: this.stateMachineDef
            }
        });
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt || !opt.json || !this.stateMachineDef.id) {
                return;
            }
            this.isLoading = true;
            const action = startLaunch({ id: this.id, input: opt.json, parameters: opt.parameters });
            this.store.dispatch(action);
        });
    }
    init() {
        const action = startGetJson({ id: this.id });
        this.store.dispatch(action);
    }
};
EditStateMachineComponent = __decorate([
    Component({
        selector: 'edit-state-machine',
        templateUrl: './edit.component.html',
        styleUrls: ['./edit.component.scss']
    })
], EditStateMachineComponent);
export { EditStateMachineComponent };
//# sourceMappingURL=edit.component.js.map