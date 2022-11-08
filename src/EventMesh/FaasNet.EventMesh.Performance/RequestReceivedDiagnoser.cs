using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace FaasNet.EventMesh.Performance
{
    public class RequestReceivedDiagnoser : IDiagnoser
    {
        public IEnumerable<string> Ids => new[] { nameof(RequestReceivedDiagnoser) };

        public IEnumerable<IExporter> Exporters => Array.Empty<IExporter>();

        public IEnumerable<IAnalyser> Analysers => Array.Empty<IAnalyser>();

        public void DisplayResults(BenchmarkDotNet.Loggers.ILogger logger) { }

        public RunMode GetRunMode(BenchmarkCase benchmarkCase)
        {
            return RunMode.NoOverhead;
        }

        public void Handle(HostSignal signal, DiagnoserActionParameters parameters) 
        {
        }

        public IEnumerable<Metric> ProcessResults(DiagnoserResults results)
        {
            yield return new Metric(new CustomMetricDescriptor(), 1);
        }

        public bool RequiresBlockingAcknowledgments(BenchmarkCase benchmarkCase)
        {
            return false;
        }

        public IEnumerable<ValidationError> Validate(ValidationParameters validationParameters) => Array.Empty<ValidationError>();
    }

    public class CustomMetricDescriptor : IMetricDescriptor
    {
        public string Id => "Id";

        public string DisplayName => "Id";

        public string Legend => String.Empty;

        public string NumberFormat => null;

        public UnitType UnitType => UnitType.Size;

        public string Unit => String.Empty;

        public bool TheGreaterTheBetter => false;

        public int PriorityInCategory => 0;
    }
}
