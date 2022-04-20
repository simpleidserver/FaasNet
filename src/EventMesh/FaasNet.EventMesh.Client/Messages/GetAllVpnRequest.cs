namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnRequest : Package
    {
        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
        }

        public override string ToString()
        {
            return $"Command = {Header.Command}";
        }
    }
}
