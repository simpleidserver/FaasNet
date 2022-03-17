import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startDelete } from '@stores/functions/actions/function.actions';
import { filter } from 'rxjs/operators';
let InfoFunctionComponent = class InfoFunctionComponent {
    constructor(store, activatedRoute, actions$, translateService, snackBar, router) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.router = router;
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[Functions] COMPLETE_DELETE_FUNCTION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('functions.messages.functionRemoved'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.router.navigate(['/functions']);
        });
        this.actions$.pipe(filter((action) => action.type === '[Functions] ERROR_DELETE_FUNCTION'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('functions.messages.errorRemoveFunction'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.function = state;
        });
    }
    ngOnDestroy() {
    }
    delete() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = startDelete({ name: name });
        this.store.dispatch(action);
    }
};
InfoFunctionComponent = __decorate([
    Component({
        selector: 'info-function',
        templateUrl: './info.component.html'
    })
], InfoFunctionComponent);
export { InfoFunctionComponent };
//# sourceMappingURL=info.component.js.map