using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace FaasNet.EventMesh.Performance
{
    public class RequestSentColumn : IColumn
    {
        public string Id => nameof(RequestSentColumn);

        public string ColumnName => "NbRequestSent";

        public bool AlwaysShow => true;

        public ColumnCategory Category => ColumnCategory.Statistics;

        public int PriorityInCategory => 0;

        public bool IsNumeric => true;

        public UnitType UnitType => UnitType.Size;

        public string Legend => "Number of outgoing requests";

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
            if (!File.Exists(Constants.FilePath)) return string.Empty;
            var content = File.ReadAllText(Constants.FilePath);
            return content.Split(';').First();
        }

        public bool IsAvailable(Summary summary) => true;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => true;
    }
}
