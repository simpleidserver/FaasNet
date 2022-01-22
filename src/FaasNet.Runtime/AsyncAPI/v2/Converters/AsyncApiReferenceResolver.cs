using FaasNet.Runtime.AsyncAPI.v2.Models;
using FaasNet.Runtime.JSchemas;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.v2.Converters
{
    public class AsyncApiReferenceResolver : FaasNetReferenceResolver
    {
        private Dictionary<MessageReference, string> _unprocessedMessageReferences;

        public AsyncApiReferenceResolver()
        {
            _unprocessedMessageReferences = new Dictionary<MessageReference, string>();
        }

        public override void AddReference(object context, string reference, object value)
        {
            base.AddReference(context, reference, value);
            var message = value as Message;
            if (message != null)
            {
                ResolveUnprocessedMessageReferences(reference, message);
            }
        }

        public void AddUnprocessedMessageReference(MessageReference messageReference, string reference)
        {
            _unprocessedMessageReferences.Add(messageReference, reference);
        }

        private void ResolveUnprocessedMessageReferences(string reference, Message value)
        {
            foreach (var kvp in _unprocessedMessageReferences)
            {
                if (kvp.Value == reference)
                {
                    kvp.Key.Reference = value;
                }
            }
        }
    }
}
