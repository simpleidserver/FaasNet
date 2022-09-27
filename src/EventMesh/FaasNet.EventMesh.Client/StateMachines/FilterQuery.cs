using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.StateMachines
{
	public class FilterQuery
	{
		public SortOrders SortOrder { get; set; }
		public string SortBy { get; set; }
		public int NbRecords { get; set; } = 10;
		public int Page { get; set; }
		public bool HasComparison { get; set; }
		public ComparisonExpression Comparison { get; set; }

        public void Serialize(WriteBufferContext context)
		{
			context.WriteInteger((int)SortOrder);
			context.WriteString(SortBy);
			context.WriteInteger(NbRecords);
			context.WriteInteger(Page);
			context.WriteBoolean(Comparison != null);
			if (Comparison != null) Comparison.Serialize(context);
		}

		public void Deserialize(ReadBufferContext context)
		{
			SortOrder = (SortOrders)context.NextInt();
			SortBy = context.NextString();
			NbRecords = context.NextInt();
			Page = context.NextInt();
			HasComparison = context.NextBoolean();
			if (HasComparison)
			{
				Comparison = new ComparisonExpression();
				Comparison.Deserialize(context);
			}
		}
    }

	public enum ComparisonOperators
	{
		EQUAL = 1
	}

	public class ComparisonExpression
	{
		public ComparisonOperators Operator { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }

		public void Serialize(WriteBufferContext context)
		{
			context.WriteInteger((int)Operator);
			context.WriteString(Key);
			context.WriteString(Value);
		}

		public void Deserialize(ReadBufferContext context)
		{
			Operator = (ComparisonOperators)context.NextInt();
			Key = context.NextString();
			Value = context.NextString();
		}
    }
}
