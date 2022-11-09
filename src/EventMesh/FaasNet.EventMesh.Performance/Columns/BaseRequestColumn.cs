using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace FaasNet.EventMesh.Performance.Columns
{
    public abstract class BaseRequestColumn : IColumn
    {
        public abstract string Id { get; }

        public abstract string ColumnName { get; }

        public bool AlwaysShow => true;

        public ColumnCategory Category => ColumnCategory.Statistics;

        public int PriorityInCategory => 0;

        public bool IsNumeric => true;

        public UnitType UnitType => UnitType.Size;

        public abstract string Legend { get; }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
            if (!File.Exists(Constants.SummaryFilePath)) return string.Empty;
            var content = File.ReadAllText(Constants.SummaryFilePath);
            return GetValue(content.Split(Constants.Separator));
        }

        public bool IsAvailable(Summary summary) => true;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

        protected abstract string GetValue(IEnumerable<string> values);
    }
}
