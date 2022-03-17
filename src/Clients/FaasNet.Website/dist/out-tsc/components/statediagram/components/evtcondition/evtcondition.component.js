import { __decorate } from "tslib";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
let EvtConditionComponent = class EvtConditionComponent {
    constructor() {
        this.eventRefSubscription = null;
        this._evtCondition = null;
        this._transition = null;
        this.closed = new EventEmitter();
        this.updateEvtConditionFormGroup = new FormGroup({
            eventRef: new FormControl()
        });
    }
    get transition() {
        return this._transition;
    }
    set transition(v) {
        this._transition = v;
        this._evtCondition = v;
        this.init();
    }
    ngOnInit() {
    }
    ngOnDestroy() {
        if (this.eventRefSubscription) {
            this.eventRefSubscription.unsubscribe();
        }
    }
    close() {
        this.closed.emit();
    }
    init() {
        var _a, _b, _c;
        const self = this;
        this.ngOnDestroy();
        (_a = this.updateEvtConditionFormGroup.get('eventRef')) === null || _a === void 0 ? void 0 : _a.setValue((_b = this._evtCondition) === null || _b === void 0 ? void 0 : _b.eventRef);
        this.eventRefSubscription = (_c = this.updateEvtConditionFormGroup.get('eventRef')) === null || _c === void 0 ? void 0 : _c.valueChanges.subscribe((e) => {
            if (self._evtCondition) {
                self._evtCondition.eventRef = e;
            }
        });
    }
};
__decorate([
    Output()
], EvtConditionComponent.prototype, "closed", void 0);
__decorate([
    Input()
], EvtConditionComponent.prototype, "transition", null);
EvtConditionComponent = __decorate([
    Component({
        selector: 'evtcondition-editor',
        templateUrl: './evtcondition.component.html',
        styleUrls: ['../state-editor.component.scss']
    })
], EvtConditionComponent);
export { EvtConditionComponent };
//# sourceMappingURL=evtcondition.component.js.map