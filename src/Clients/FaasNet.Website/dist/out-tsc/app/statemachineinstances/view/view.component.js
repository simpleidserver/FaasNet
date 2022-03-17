import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGet } from '@stores/statemachineinstances/actions/statemachineinstances.actions';
import { StateMachineInstanceDetails } from '@stores/statemachineinstances/models/statemachineinstance-details.model';
import { startGetJson } from '@stores/statemachines/actions/statemachines.actions';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { filter } from 'rxjs/operators';
let ViewStateMachineInstanceComponent = class ViewStateMachineInstanceComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar, dialog) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.dialog = dialog;
        this.stateMachineDef = new StateMachineModel();
        this.stateMachineInstance = new StateMachineInstanceDetails();
        this.isLoading = false;
    }
    ngOnInit() {
        const self = this;
        self.actions$.pipe(filter((action) => action.type === '[StateMachines] ERROR_GET_JSON_STATE_MACHINE'))
            .subscribe(() => {
            self.isLoading = false;
            self.snackBar.open(this.translateService.instant('stateMachines.messages.errorGetStateMachine'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        self.actions$.pipe(filter((action) => action.type === '[StateMachineInstances] ERROR_GET_STATEMACHINE_INSTANCE'))
            .subscribe(() => {
            self.isLoading = false;
            self.snackBar.open(this.translateService.instant('stateMachineInstance.messages.errorGetStateMachineInstance'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectStateMachineInstanceResult)).subscribe((stateMachineInstance) => {
            if (!stateMachineInstance) {
                return;
            }
            self.stateMachineInstance = stateMachineInstance;
            self.refreshLoading();
        });
        this.store.pipe(select(fromReducers.selectStateMachineResult)).subscribe((stateMachine) => {
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
    refreshLoading() {
        if (this.stateMachineDef && this.stateMachineInstance && this.stateMachineDef.id && this.stateMachineInstance.id) {
            this.isLoading = false;
        }
    }
    init() {
        this.isLoading = true;
        const id = this.activatedRoute.snapshot.params['id'];
        const instid = this.activatedRoute.snapshot.params['instid'];
        const startGetDef = startGetJson({ id: id });
        const startGetInstance = startGet({ id: instid });
        this.store.dispatch(startGetDef);
        this.store.dispatch(startGetInstance);
    }
};
ViewStateMachineInstanceComponent = __decorate([
    Component({
        selector: 'view-state-machineinstance',
        templateUrl: './view.component.html',
        styleUrls: ['./view.component.scss']
    })
], ViewStateMachineInstanceComponent);
export { ViewStateMachineInstanceComponent };
//# sourceMappingURL=view.component.js.map