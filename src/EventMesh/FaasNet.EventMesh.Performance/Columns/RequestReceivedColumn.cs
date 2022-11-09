namespace FaasNet.EventMesh.Performance.Columns
{
    public class RequestReceivedColumn : BaseRequestColumn
    {
        public override string Id => nameof(RequestReceivedColumn);

        public override string ColumnName => "NbRequestReceived";

        public override string Legend => "Number of received requests";

        protected override string GetValue(IEnumerable<string> values) => values.Last();
    }
}
