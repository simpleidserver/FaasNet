using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using EventMesh.Runtime.Messages;
using System.Linq;

namespace EventMesh.Runtime.Extensions
{
    public static class CloudEventExtensions
    {
        public static void Serialize(this CloudEvent cloudEvent, WriteBufferContext bufferContext)
        {
            var formatter = new JsonEventFormatter();
            var binary = formatter.EncodeBinaryModeEventData(cloudEvent).ToArray();
            bufferContext.WriteByteArray(binary);
            var attrs = cloudEvent.GetPopulatedAttributes();
            bufferContext.WriteInteger(attrs.Count());
            foreach(var property in cloudEvent.GetPopulatedAttributes())
            {
                bufferContext.WriteString(property.Key.Name);
                bufferContext.WriteString(property.Key.Type.Name);
                bufferContext.WriteString(property.Value.ToString());
            }
        }

        public static CloudEvent DeserializeCloudEvent(this ReadBufferContext context)
        {
            var cloudEventPayload = context.NextByteArray();
            var cloudEvt = new CloudEvent();
            var formatter = new JsonEventFormatter();
            formatter.DecodeBinaryModeEventData(cloudEventPayload, cloudEvt);
            int nbAttributes = context.NextInt();
            for (int i = 0; i < nbAttributes; i++)
            {
                var attrName = context.NextString();
                context.NextString();
                var attrValue = context.NextString();
                cloudEvt.SetAttributeFromString(attrName, attrValue);
            }

            return cloudEvt;
        }
    }
}
