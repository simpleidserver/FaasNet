namespace OpenTelemetry.Metrics
{
    internal static class PeriodicExportingMetricReaderHelper
    {
        internal static PeriodicExportingMetricReader CreatePeriodicExportingMetricReader(
            BaseExporter<Metric> exporter,
            MetricReaderOptions options,
            int defaultExportIntervalMilliseconds,
            int defaultExportTimeoutMilliseconds)
        {
            var exportInterval =
                options.PeriodicExportingMetricReaderOptions?.ExportIntervalMilliseconds
                ?? defaultExportIntervalMilliseconds;

            var exportTimeout =
                options.PeriodicExportingMetricReaderOptions?.ExportTimeoutMilliseconds
                ?? defaultExportTimeoutMilliseconds;

            var metricReader = new PeriodicExportingMetricReader(exporter, exportInterval, exportTimeout)
            {
                TemporalityPreference = options.TemporalityPreference,
            };

            return metricReader;
        }
    }
}