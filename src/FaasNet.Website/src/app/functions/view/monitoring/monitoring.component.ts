import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { PrometheusQueryRangeResult } from '@stores/common/prometheus-queryrange-result.model';
import { startDelete, startGetThreads, startGetVirtualMemoryBytes } from '@stores/functions/actions/function.actions';
import { ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { Subscription } from 'rxjs';

@Component({
  selector: 'monitoring-function',
  templateUrl: './monitoring.component.html'
})
export class MonitoringFunctionComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  threadValues: ChartDataSets[] = [];
  threadLabels: Label[] = [];
  virtualMemoryBytesValues: ChartDataSets[] = [];
  virtualMemoryBytesLabels: Label[] = [];
  options = {
    elements: {
      point: {
        radius: 0
      }
    },
    scales: {
      xAxes: [{
        ticks: {
          userCallback: function (item: any, index: any) {
            if (index === 0 || (((index + 1) % 15)) === 0) return item;
          },
          autoSkip: false
        }
      }]
    }
  };
  name: string | undefined;
  intervalRefreshThreads: any;


  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    // sum(rate(process_cpu_seconds_total{job="test2"}[5m])) * 100
    const self = this;
    this.threadValues = [
      { data: [], label: this.translateService.instant('functions.numberThreads') }
    ];
    this.virtualMemoryBytesValues = [
      { data: [], label: this.translateService.instant('functions.virtualMemoryBytes') }
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
    this.subscription = this.activatedRoute.params.subscribe(() => {
      this.refresh();
    });
    this.intervalRefreshThreads = setInterval(this.refresh.bind(self), 3000);
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
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = startDelete({ name: name });
    this.store.dispatch(action);
  }

  private refreshLineChart(values: any, labels: Label[], value: ChartDataSets) {
    let minDate = values[0].length === 0 ? null : values[0][0];
    let maxDate = labels.length === 0 ? null :labels[labels.length - 1];
    let removeThreads = labels.filter((str: any) => {
      if (!minDate || (minDate && this.formatDate(str) > minDate)) {
        return false;
      }

      return true;
    });
    let addThreads = values.filter((str: any) => {
      if (!maxDate || (maxDate && this.formatDate(str[0]) > maxDate.toString())) {
        return true;
      }

      return false;
    });
    removeThreads.forEach((r: any) => {
      const index = labels.indexOf(r);
      value.data?.splice(index, 1);
      labels.splice(index, 1);
    });
    addThreads.forEach((r: any) => {
      const d = new Date(r[0] * 1000);
      const label = d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
      value.data?.push(r[1]);
      labels.push(label);
    });
  }

  private refresh() {
    this.refreshThreads();
    this.refreshVirtualMemoryBytes();
  }

  private refreshThreads() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    this.name = name;
    let endDate = new Date();
    let startDate = new Date();
    startDate.setHours(startDate.getHours() - 1);
    const action = startGetThreads({ name: name, startDate: startDate.getTime() / 1000, endDate: endDate.getTime() / 1000 });
    this.store.dispatch(action);
  }

  private refreshVirtualMemoryBytes() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    this.name = name;
    let endDate = new Date();
    let startDate = new Date();
    startDate.setHours(startDate.getHours() - 1);
    const action = startGetVirtualMemoryBytes({ name: name, startDate: startDate.getTime() / 1000, endDate: endDate.getTime() / 1000 });
    this.store.dispatch(action);
  }

  private formatDate(str: any) : string {
    const d = new Date(str[0] * 1000);
    return d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
  }
}
