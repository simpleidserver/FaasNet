import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetConfiguration, startInvoke } from '@stores/functions/actions/function.actions';
import { filter } from 'rxjs/operators';
let InvokeFunctionComponent = class InvokeFunctionComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.form = new FormGroup({});
        this.inputForm = new FormControl();
        this.output = {};
    }
    ngOnInit() {
        this.form = new FormGroup({});
        this.inputForm = new FormControl();
        this.actions$.pipe(filter((action) => action.type === '[Functions] COMPLETE_INVOKE_FUNCTION'))
            .subscribe((e) => {
            this.output = e.content;
        });
        this.actions$.pipe(filter((action) => action.type === '[Functions] ERROR_INVOKE_FUNCTION'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('functions.messages.errorInvokeFunction'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectFunctionConfigurationResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.option = state;
        });
        this.subscription = this.activatedRoute.params.subscribe(() => {
            this.refresh();
        });
    }
    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
    refresh() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = startGetConfiguration({ name: name });
        this.store.dispatch(action);
    }
    onSave(evt) {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const request = {
            configuration: this.form.value,
            input: JSON.parse(this.inputForm.value)
        };
        const invoke = startInvoke({ name: name, request: request });
        this.store.dispatch(invoke);
    }
};
InvokeFunctionComponent = __decorate([
    Component({
        selector: 'invoke-function',
        templateUrl: './invoke.component.html'
    })
], InvokeFunctionComponent);
export { InvokeFunctionComponent };
//# sourceMappingURL=invoke.component.js.map