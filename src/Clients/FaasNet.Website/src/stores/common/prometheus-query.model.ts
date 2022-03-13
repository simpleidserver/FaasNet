class BasePrometheusQueryResult<T> {
  status: string | undefined;
  data: PrometheusDataResult<T> | undefined;
}

export class PrometheusDataResult<T> {
  constructor() {
    this.result = [];
  }

  resultType: string | undefined;
  result: T[];
}

export class PrometheusRecordRangeResult {
  metric: PrometheusMetricResult | undefined;
  values: [[]] | undefined;
}

export class PrometheusRecordResult {
  metric: PrometheusMetricResult | undefined;
  value: any[] | undefined;
}


export class PrometheusMetricResult {
  instance: string | undefined;
  job: string | undefined;
}


export class PrometheusQueryRangeResult extends BasePrometheusQueryResult<PrometheusRecordRangeResult>{
}

export class PrometheusQueryResult extends BasePrometheusQueryResult<PrometheusRecordResult>{
}
