import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { PrometheusQueryRangeResult, PrometheusQueryResult } from '@stores/common/prometheus-query.model';
import { startDelete, startGetCpuUsage, startGetDetails, startGetRequestDuration, startGetThreads, startGetTotalRequests, startGetVirtualMemoryBytes } from '@stores/functions/actions/function.actions';
import { FunctionDetailsResult } from '@stores/functions/models/function-details.model';
import { ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';

@Component({
  selector: 'monitoring-function',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringFunctionComponent implements OnInit, OnDestroy {
  threadValues: ChartDataSets[] = [];
  threadLabels: Label[] = [];
  virtualMemoryBytesValues: ChartDataSets[] = [];
  virtualMemoryBytesLabels: Label[] = [];
  cpuUsageValues: ChartDataSets[] = [];
  cpuUsageLabels: Label[] = [];
  requestDurationValues: ChartDataSets[] = [];
  requestDurationLabels: Label[] = [];
  options = {
    elements: {
      point: {
        radius: 0
      }
    }
  };
  intervalRefreshThreads: any;
  updateMonitoringStatusFormGroup: FormGroup = new FormGroup({
    range: new FormControl(),
    duration: new FormControl()
  });
  nbPods: number = 0;
  totalRequests: string = "0";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService) { }

  ngOnInit(): void {
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
    this.store.pipe(select(fromReducers.selectFunctionThreadsResult)).subscribe((state: PrometheusQueryRangeResult | null) => {
      if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
        return;
      }

      const firstResult = state.data.result[0];
      const values: any = firstResult.values;
      this.refreshLineChart(values, this.threadLabels, this.threadValues[0]);
    });
    this.store.pipe(select(fromReducers.selectFunctionVirtualMemoryBytesResult)).subscribe((state: PrometheusQueryRangeResult | null) => {
      if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
        return;
      }

      const firstResult = state.data.result[0];
      const values: any = firstResult.values;
      this.refreshLineChart(values, this.virtualMemoryBytesLabels, this.virtualMemoryBytesValues[0]);
    });
    this.store.pipe(select(fromReducers.selectCpuUsageResult)).subscribe((state: PrometheusQueryRangeResult | null) => {
      if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
        return;
      }

      const firstResult = state.data.result[0];
      const values: any = firstResult.values;
      this.refreshLineChart(values, this.cpuUsageLabels, this.cpuUsageValues[0]);
    });
    this.store.pipe(select(fromReducers.selectRequestDurationResult)).subscribe((state: PrometheusQueryRangeResult | null) => {
      if (!state || !state.data || !state.data.result || state.data.result.length !== 1) {
        return;
      }

      const firstResult = state.data.result[0];
      const values: any = firstResult.values;
      this.refreshLineChart(values, this.requestDurationLabels, this.requestDurationValues[0]);
    });
    this.store.pipe(select(fromReducers.selectDetailsResult)).subscribe((state: FunctionDetailsResult | null) => {
      if (!state || !state.pods || state.pods.length === 0) {
        return;
      }

      this.nbPods = state.pods.length;
    });
    this.store.pipe(select(fromReducers.selectTotalRequests)).subscribe((state: PrometheusQueryResult | null) => {
      if (!state || !state.data || !state.data.result || state.data.result.length !== 1 || !state.data.result[0].value) {
        return;
      }

      this.totalRequests = state.data.result[0].value[1];
    });
    this.intervalRefreshThreads = setInterval(this.refresh.bind(self), 1000);
    this.updateMonitoringStatusFormGroup.get('range')?.setValue('15');
    this.updateMonitoringStatusFormGroup.get('duration')?.setValue('5');
  }

  ngOnDestroy() {
    if (this.intervalRefreshThreads) {
      clearInterval(this.intervalRefreshThreads);
    }
  }

  delete() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startDelete({ name: name });
    this.store.dispatch(action);
  }

  private refreshLineChart(values: any, labels: Label[], value: ChartDataSets) {
    if (!values || !value.data) {
      return;
    }

    value.data.length = 0;
    labels.length = 0;
    values.forEach((r: any) => {
      const d = new Date(r[0] * 1000);
      const label = d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
      value.data?.push(r[1]);
      labels.push(label);
    });
  }

  private refresh() {
    this.refreshThreads();
    this.refreshVirtualMemoryBytes();
    this.refreshCpuUsage();
    this.refreshRequestDuration();
    this.refreshDetails();
    this.refreshTotalRequests();
  }

  private refreshThreads() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    let startDate = this.getStartDate();
    let endDate = this.getEndDate();
    const action = startGetThreads({ name: name, startDate: startDate, endDate: endDate });
    this.store.dispatch(action);
  }

  private refreshVirtualMemoryBytes() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    let startDate = this.getStartDate();
    let endDate = this.getEndDate();
    const action = startGetVirtualMemoryBytes({ name: name, startDate: startDate, endDate: endDate });
    this.store.dispatch(action);
  }

  private refreshCpuUsage() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    let startDate = this.getStartDate();
    let endDate = this.getEndDate();
    const action = startGetCpuUsage({ name: name, startDate: startDate, endDate: endDate, duration: this.getDuration() });
    this.store.dispatch(action);
  }

  private refreshRequestDuration() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    let startDate = this.getStartDate();
    let endDate = this.getEndDate();
    const action = startGetRequestDuration({ name: name, startDate: startDate, endDate: endDate, duration: this.getDuration() });
    this.store.dispatch(action);
  }

  private refreshDetails() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startGetDetails({ name: name});
    this.store.dispatch(action);
  }

  private refreshTotalRequests() {
    let endDate = this.getEndDate();
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startGetTotalRequests({ name: name, time: endDate });
    this.store.dispatch(action);
  }

  private getEndDate() {
    var result = new Date();
    return result.getTime() / 1000;
  }

  private getStartDate() {
    var result = new Date();
    result.setMinutes(result.getMinutes() - this.getRange());
    return result.getTime() / 1000;
  }

  private getRange() {
    return parseInt(this.updateMonitoringStatusFormGroup.get('range')?.value);
  }

  private getDuration() {
    return parseInt(this.updateMonitoringStatusFormGroup.get('duration')?.value);
  }
}
