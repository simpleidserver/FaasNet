﻿using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteRequest : BaseConsensusPackage
    {
        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}