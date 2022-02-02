using System;

namespace EventMesh.Runtime.Messages
{
    public class EventMeshHeader
    {
        public EventMeshHeader(EventMeshCommands command, EventMeshHeaderStatus status, string seq)
        {
            Command = command;
            Status = status;
            Seq = seq;
        }

        public EventMeshCommands Command { get; set; }
        public EventMeshHeaderStatus Status { get; set; }
        public string Seq { get; set; }

        public virtual void Serialize(EventMeshWriterBufferContext context)
        {
            Command.Serialize(context);
            Status.Serialize(context);
            context.WriteString(Seq);
        }

        public static EventMeshHeader Deserialize(EventMeshReaderBufferContext context)
        {
            var cmd = EventMeshCommands.Deserialize(context);
            var status = EventMeshHeaderStatus.Deserialize(context);
            var seq = context.NextString();
            return new EventMeshHeader(cmd, status, seq);
        }
    }
}
