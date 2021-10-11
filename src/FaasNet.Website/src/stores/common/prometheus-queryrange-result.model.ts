export class PrometheusQueryRangeResult {
  status: string | undefined;
  data: PrometheusDataResult | undefined;
}

export class PrometheusDataResult {
  constructor() {
    this.result = [];
  }

  resultType: string | undefined;
  result: PrometheusRecordResult[];
}

export class PrometheusRecordResult {
  metric: PrometheusMetricResult | undefined;
  values: [[]] | undefined;
}

export class PrometheusMetricResult {
  instance: string | undefined;
  job: string | undefined;
}
