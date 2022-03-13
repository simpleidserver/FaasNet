import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import { startGet, startUpdateUIOperation } from '@stores/apis/actions/api.actions';
import * as fromReducers from '@stores/appstate';
import { FunctionResult } from '@stores/functions/models/function.model';
import { filter } from 'rxjs/operators';
import { FunctionModel, FunctionRecord } from './function-panel/function.model';
import { LaunchFunctionDialogComponent } from './launch-function-dialog.component';
class EditorModel {
    constructor() {
        this.index = 0;
        this.model = null;
    }
}
let EditApiComponent = class EditApiComponent {
    constructor(dialog, store, activatedRoute, actions$, snackBar, translateService) {
        this.dialog = dialog;
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.actions$ = actions$;
        this.snackBar = snackBar;
        this.translateService = translateService;
        this.editor = null;
        this.selectedNode = null;
        this.selectedIndex = 0;
        this.currentIndex = 0;
        this.nodes = [];
        this.name = "";
        this.opName = "";
        this.isInitialized = false;
        this.currentOperation = null;
    }
    ngOnInit() {
        this.mouseupHandler = this.handleMouseUp.bind(this);
        this.actions$.pipe(filter((action) => action.type === '[ApiDefs] COMPLETE_UPDATE_UI_OPERATION'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('apis.messages.apiOperationUpdated'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.refresh();
        });
        this.actions$.pipe(filter((action) => action.type === '[ApiDefs] ERROR_UPDATE_UI_OPERATION'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('apis.messages.errorUpdateApiOperation'), this.translateService.instant('undo'), {
                duration: 2000
            });
        });
        this.nodes = [];
        this.refresh();
    }
    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
        if (this.secondSubscription) {
            this.secondSubscription.unsubscribe();
        }
        window.removeEventListener('mouseup', this.mouseupHandler);
        this.destroy();
    }
    ngAfterViewInit() {
        var spacing_x = 20;
        var spacing_y = 50;
        window.addEventListener('mouseup', this.mouseupHandler);
        this.secondSubscription = this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state) => {
            if (!state || !this.opName || this.isInitialized) {
                return;
            }
            this.isInitialized = true;
            flowy(document.getElementById("canvas"), null, null, this.snapping.bind(this), null, spacing_x, spacing_y);
            this.currentOperation = state.operations.filter(o => o.name === this.opName)[0];
            if (this.currentOperation.ui) {
                flowy.import(this.currentOperation.ui);
                this.currentOperation.ui.blocks.forEach((b) => {
                    var _a;
                    const model = new FunctionModel(new FunctionRecord());
                    const blockId = b.data.filter((d) => d.name === 'blockid')[0].value;
                    if (b.model && model.content) {
                        model.content.configuration = b.model.configuration;
                    }
                    if (b.model && ((_a = b.model.info) === null || _a === void 0 ? void 0 : _a.name) && model.content) {
                        model.content.info = new FunctionResult();
                        model.content.info.name = b.model.info.name;
                    }
                    this.nodes.push({ index: parseInt(blockId), model: model });
                });
                const indexes = this.nodes.map(n => n.index);
                this.currentIndex = Math.max.apply(null, indexes) + 1;
            }
        });
    }
    snapping(b) {
        const self = this;
        const type = b.querySelector('.blockelemtype').value;
        b.setAttribute('data-index', self.currentIndex);
        b.setAttribute('data-type', type);
        switch (type) {
            case 'function':
                const data = new FunctionModel(new FunctionRecord());
                this.nodes.push({ index: self.currentIndex, model: data });
                break;
        }
        this.currentIndex++;
        return true;
    }
    close() {
        const node = document.querySelector("div[data-index='" + this.selectedIndex + "']");
        node.classList.remove('isselected');
        this.selectedNode = null;
        this.selectedIndex = 0;
    }
    launch() {
        var _a;
        this.dialog.open(LaunchFunctionDialogComponent, {
            width: '800px',
            data: {
                funcName: this.name,
                opName: (_a = this.currentOperation) === null || _a === void 0 ? void 0 : _a.path
            }
        });
    }
    save() {
        var _a, _b;
        const json = flowy.output();
        json.blocks.forEach((b) => {
            var _a;
            const blockId = b.data.filter((r) => r.name === 'blockid')[0].value;
            const filtered = this.nodes.filter((n) => n.index == parseInt(blockId));
            if (filtered.length === 1) {
                b.model = (_a = filtered[0].model) === null || _a === void 0 ? void 0 : _a.content;
            }
        });
        json.html = json.html.replace('isselected', '');
        const name = (_a = this.activatedRoute) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const opName = (_b = this.activatedRoute) === null || _b === void 0 ? void 0 : _b.snapshot.params['opname'];
        const updateUIOperation = startUpdateUIOperation({ funcName: name, operationName: opName, ui: json });
        this.store.dispatch(updateUIOperation);
    }
    refresh() {
        var _a, _b;
        this.name = (_a = this.activatedRoute) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.opName = (_b = this.activatedRoute) === null || _b === void 0 ? void 0 : _b.snapshot.params['opname'];
        const action = startGet({ funcName: this.name });
        this.store.dispatch(action);
    }
    destroy() {
        if (flowy.destroy) {
            flowy.destroy();
        }
    }
    handleMouseUp(evt) {
        const parent = evt.target.closest('.blockelem.block');
        if (!parent || parent.classList.contains('isselected')) {
            return;
        }
        const children = document.querySelectorAll('.blockelem.block');
        children.forEach(function (c) {
            c.classList.remove('isselected');
        });
        parent.classList.add('isselected');
        this.selectedIndex = parent.getAttribute('data-index');
        const filtered = this.nodes.filter((n) => n.index == this.selectedIndex);
        if (filtered.length === 0) {
            return;
        }
        this.selectedNode = filtered[0].model;
    }
};
EditApiComponent = __decorate([
    Component({
        selector: 'edit-api',
        templateUrl: './edit.component.html',
        styleUrls: ['./edit.component.scss']
    })
], EditApiComponent);
export { EditApiComponent };
//# sourceMappingURL=edit.component.js.map