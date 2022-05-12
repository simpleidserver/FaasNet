
namespace FaasNet.EventMesh.Client.Messages
{
    public class Package
    {
        private const string MAGIC_CODE = "EventMesh";
        private const string PROTOCOL_VERSION = "0000";

        public Header Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static Package Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            var header = Header.Deserialize(context);
            if (Commands.HELLO_REQUEST == header.Command)
            {
                var result = new HelloRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.SUBSCRIBE_REQUEST == header.Command)
            {
                var result = new SubscriptionRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.ADD_CLIENT_REQUEST == header.Command)
            {
                var result = new AddClientRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if(Commands.ADD_CLIENT_RESPONSE == header.Command)
            {
                var result = new AddClientResponse
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.SUBSCRIBE_RESPONSE == header.Command)
            {
                var result = new SubscriptionResult
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.ADD_BRIDGE_REQUEST == header.Command)
            {
                var result = new AddBridgeRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.DISCONNECT_REQUEST == header.Command)
            {
                var result = new DisconnectRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }
            
            if (Commands.HELLO_RESPONSE == header.Command)
            {
                var result = new HelloResponse
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.PUBLISH_MESSAGE_REQUEST == header.Command)
            {
                var result = new PublishMessageRequest
                {
                    Header= header
                };
                result.Extract(context);
                return result;
            }

            if(Commands.GET_ALL_VPNS_RESPONSE == header.Command)
            {
                var result = new GetAllVpnResponse
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if(Commands.ADD_VPN_REQUEST == header.Command)
            {
                var result = new AddVpnRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            return new Package
            {
                Header = header
            };
        }
    }
}
