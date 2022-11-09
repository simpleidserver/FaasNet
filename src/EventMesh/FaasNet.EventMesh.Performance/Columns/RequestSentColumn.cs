namespace FaasNet.EventMesh.Performance.Columns
{
    public class RequestSentColumn : BaseRequestColumn
    {
        public override string Id => nameof(RequestSentColumn);

        public override string ColumnName => "NbRequestSent";

        public override string Legend => "Number of received sent";

        protected override string GetValue(IEnumerable<string> values) => values.First();
    }
}
