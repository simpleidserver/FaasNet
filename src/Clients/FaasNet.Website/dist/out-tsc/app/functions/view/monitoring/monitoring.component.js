import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { select } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startDelete, startGetCpuUsage, startGetDetails, startGetRequestDuration, startGetThreads, startGetTotalRequests, startGetVirtualMemoryBytes } from '@stores/functions/actions/function.actions';
let MonitoringFunctionComponent = class MonitoringFunctionComponent {
    constructor(store, activatedRoute, translateService) {
        this.store = store;
        this.activatedRoute = activatedRoute;
        this.translateService = translateService;
        this.threadValues = [];
        this.threadLabels = [];
        this.virtualMemoryBytesValues = [];
        this.virtualMemoryBytesLabels = [];
        this.cpuUsageValues = [];
        this.cpuUsageLabels = [];
        this.requestDurationValues = [];
        this.requestDurationLabels = [];
        this.options = {
            elements: {
                point: {
                    radius: 0
                }
            }
        };
        this.updateMonitoringStatusFormGroup = new FormGroup({
            range: new FormControl(),
            duration: new FormControl()
        });
        this.nbPods = 0;
        this.totalRequests = "0";
    }
    ngOnInit() {
        var _a, _b;
        const self = this;
        this.threadValues = [
            { data: [], label: this.translateService.instant('functions.numberThreads') }
        ];
        this.virtualMemoryBytesValues = [
            { data: [], label: this.translateService.instant('functions.virtualMemoryBytes') }
        ];
        this.cpuUsageValues = [
            { data: [], label: this.translateService.instant('functions.cpuUsage') }
        ];
        this.requestDurationValues = [
            { data: [], label: this.translateService.instant('functions.requestDurationLegend') }
        ];
        this.store.pipe(select(fromReducers.selectFunctionThreadsResult)).subscribe((state) => {
            if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
                return;
            }
            const firstResult = state.data.result[0];
            const values = firstResult.values;
            this.refreshLineChart(values, this.threadLabels, this.threadValues[0]);
        });
        this.store.pipe(select(fromReducers.selectFunctionVirtualMemoryBytesResult)).subscribe((state) => {
            if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
                return;
            }
            const firstResult = state.data.result[0];
            const values = firstResult.values;
            this.refreshLineChart(values, this.virtualMemoryBytesLabels, this.virtualMemoryBytesValues[0]);
        });
        this.store.pipe(select(fromReducers.selectCpuUsageResult)).subscribe((state) => {
            if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
                return;
            }
            const firstResult = state.data.result[0];
            const values = firstResult.values;
            this.refreshLineChart(values, this.cpuUsageLabels, this.cpuUsageValues[0]);
        });
        this.store.pipe(select(fromReducers.selectRequestDurationResult)).subscribe((state) => {
            if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
                return;
            }
            const firstResult = state.data.result[0];
            const values = firstResult.values;
            this.refreshLineChart(values, this.requestDurationLabels, this.requestDurationValues[0]);
        });
        this.store.pipe(select(fromReducers.selectDetailsResult)).subscribe((state) => {
            if (!state || !state.pods || state.pods.length === 0) {
                return;
            }
            this.nbPods = state.pods.length;
        });
        this.store.pipe(select(fromReducers.selectTotalRequests)).subscribe((state) => {
            if (!state || !state.data || !state.data.result || state.data.result.length !== 1 || !state.data.result[0].value) {
                return;
            }
            this.totalRequests = state.data.result[0].value[1];
        });
        this.subscription = this.activatedRoute.params.subscribe(() => {
            this.refresh();
        });
        this.intervalRefreshThreads = setInterval(this.refresh.bind(self), 1000);
        (_a = this.updateMonitoringStatusFormGroup.get('range')) === null || _a === void 0 ? void 0 : _a.setValue('15');
        (_b = this.updateMonitoringStatusFormGroup.get('duration')) === null || _b === void 0 ? void 0 : _b.setValue('5');
    }
    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
        if (this.intervalRefreshThreads) {
            clearInterval(this.intervalRefreshThreads);
        }
    }
    delete() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = startDelete({ name: name });
        this.store.dispatch(action);
    }
    refreshLineChart(values, labels, value) {
        if (!values || !value.data) {
            return;
        }
        value.data.length = 0;
        labels.length = 0;
        values.forEach((r) => {
            var _a;
            const d = new Date(r[0] * 1000);
            const label = d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
            (_a = value.data) === null || _a === void 0 ? void 0 : _a.push(r[1]);
            labels.push(label);
        });
    }
    refresh() {
        this.refreshThreads();
        this.refreshVirtualMemoryBytes();
        this.refreshCpuUsage();
        this.refreshRequestDuration();
        this.refreshDetails();
        this.refreshTotalRequests();
    }
    refreshThreads() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        let startDate = this.getStartDate();
        let endDate = this.getEndDate();
        const action = startGetThreads({ name: name, startDate: startDate, endDate: endDate });
        this.store.dispatch(action);
    }
    refreshVirtualMemoryBytes() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        let startDate = this.getStartDate();
        let endDate = this.getEndDate();
        const action = startGetVirtualMemoryBytes({ name: name, startDate: startDate, endDate: endDate });
        this.store.dispatch(action);
    }
    refreshCpuUsage() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        let startDate = this.getStartDate();
        let endDate = this.getEndDate();
        const action = startGetCpuUsage({ name: name, startDate: startDate, endDate: endDate, duration: this.getDuration() });
        this.store.dispatch(action);
    }
    refreshRequestDuration() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        this.name = name;
        let startDate = this.getStartDate();
        let endDate = this.getEndDate();
        const action = startGetRequestDuration({ name: name, startDate: startDate, endDate: endDate, duration: this.getDuration() });
        this.store.dispatch(action);
    }
    refreshDetails() {
        var _a;
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = startGetDetails({ name: name });
        this.store.dispatch(action);
    }
    refreshTotalRequests() {
        var _a;
        let endDate = this.getEndDate();
        const name = (_a = this.activatedRoute.parent) === null || _a === void 0 ? void 0 : _a.snapshot.params['name'];
        const action = startGetTotalRequests({ name: name, time: endDate });
        this.store.dispatch(action);
    }
    getEndDate() {
        var result = new Date();
        return result.getTime() / 1000;
    }
    getStartDate() {
        var result = new Date();
        result.setMinutes(result.getMinutes() - this.getRange());
        return result.getTime() / 1000;
    }
    getRange() {
        var _a;
        return parseInt((_a = this.updateMonitoringStatusFormGroup.get('range')) === null || _a === void 0 ? void 0 : _a.value);
    }
    getDuration() {
        var _a;
        return parseInt((_a = this.updateMonitoringStatusFormGroup.get('duration')) === null || _a === void 0 ? void 0 : _a.value);
    }
};
MonitoringFunctionComponent = __decorate([
    Component({
        selector: 'monitoring-function',
        templateUrl: './monitoring.component.html'
    })
], MonitoringFunctionComponent);
export { MonitoringFunctionComponent };
//# sourceMappingURL=monitoring.component.js.map