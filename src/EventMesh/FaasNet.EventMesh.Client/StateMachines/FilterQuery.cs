using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.StateMachines
{
	public class FilterQuery
	{
		public SortOrders SortOrder { get; set; }
		public string SortBy { get; set; }
		public int NbRecords { get; set; } = 10;
		public int Page { get; set; }

		public void Serialize(WriteBufferContext context)
		{
			context.WriteInteger((int)SortOrder);
			context.WriteString(SortBy);
			context.WriteInteger(NbRecords);
			context.WriteInteger(Page);
		}

		public void Deserialize(ReadBufferContext context)
		{
			SortOrder = (SortOrders)context.NextInt();
			SortBy = context.NextString();
			NbRecords = context.NextInt();
			Page = context.NextInt();
		}
    }
}
