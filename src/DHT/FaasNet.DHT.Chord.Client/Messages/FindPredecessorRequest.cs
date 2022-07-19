﻿using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindPredecessorRequest : ChordPackage
    {
        public override ChordCommandTypes Command => ChordCommandTypes.FIND_PREDECESSOR_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
