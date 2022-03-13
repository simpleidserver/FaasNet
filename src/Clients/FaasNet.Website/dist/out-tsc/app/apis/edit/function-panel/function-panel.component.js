import { __decorate } from "tslib";
import { Component, Input } from "@angular/core";
import { select } from "@ngrx/store";
import * as fromReducers from '@stores/appstate';
import { startGet } from "@stores/functions/actions/function.actions";
import { AddFunctionComponent } from "./add-function.component";
import { FunctionRecord } from "./function.model";
import { UpdateFunctionConfigurationComponent } from "./update-configuration.component";
let FunctionPanelComponent = class FunctionPanelComponent {
    constructor(dialog, store, activatedRoute) {
        this.dialog = dialog;
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.isLoading = false;
        this._model = null;
    }
    get model() {
        return this._model;
    }
    set model(m) {
        var _a, _b;
        if (!m) {
            return;
        }
        this._model = m;
        this.refresh((_b = (_a = m.content) === null || _a === void 0 ? void 0 : _a.info) === null || _b === void 0 ? void 0 : _b.name);
    }
    ngOnInit() {
        this.subscription = this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state) => {
            if (!state || !this._model || !this._model.content || !this.isLoading) {
                return;
            }
            this.isLoading = false;
            this._model.content.info = state;
        });
    }
    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
    chooseFunction() {
        const dialogRef = this.dialog.open(AddFunctionComponent, {
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt || !opt.name || !this.model) {
                return;
            }
            let record = new FunctionRecord();
            record.info = opt;
            this.model.content = record;
        });
    }
    updateConfiguration() {
        var _a;
        const dialogRef = this.dialog.open(UpdateFunctionConfigurationComponent, {
            data: (_a = this.model) === null || _a === void 0 ? void 0 : _a.content,
            width: '800px'
        });
        dialogRef.afterClosed().subscribe((opt) => {
            if (!opt || !this.model || !this.model.content) {
                return;
            }
            this.model.content.configuration = opt;
        });
    }
    isSelected() {
        return this.model && this.model.content && this.model.content.info && this.model.content.info.name;
    }
    refresh(name) {
        if (!name) {
            return;
        }
        this.isLoading = true;
        const action = startGet({ name: name });
        this.store.dispatch(action);
    }
};
__decorate([
    Input()
], FunctionPanelComponent.prototype, "model", null);
FunctionPanelComponent = __decorate([
    Component({
        selector: 'function-panel',
        templateUrl: './function-panel.component.html',
        styleUrls: ['./function-panel.component.scss']
    })
], FunctionPanelComponent);
export { FunctionPanelComponent };
//# sourceMappingURL=function-panel.component.js.map