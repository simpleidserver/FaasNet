﻿using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class HelloResponse : Package
    {
        #region Properties

        public string SessionId { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(SessionId);
        }

        public void Extract(ReadBufferContext context)
        {
            SessionId = context.NextString();
        }
    }
}
