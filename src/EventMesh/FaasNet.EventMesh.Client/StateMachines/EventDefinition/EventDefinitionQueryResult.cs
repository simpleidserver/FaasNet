using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class EventDefinitionQueryResult : ISerializable
    {
        public void Deserialize(ReadBufferContext context)
        {
            throw new NotImplementedException();
        }

        public void Serialize(WriteBufferContext context)
        {
            throw new NotImplementedException();
        }
    }
}
