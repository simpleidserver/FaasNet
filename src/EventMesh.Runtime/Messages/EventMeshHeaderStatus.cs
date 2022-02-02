namespace EventMesh.Runtime.Messages
{
    public class EventMeshHeaderStatus
    {
        public static EventMeshHeaderStatus SUCCESS = new EventMeshHeaderStatus(0, "success");
        public static EventMeshHeaderStatus FAIL = new EventMeshHeaderStatus(1, "fail");
        public static EventMeshHeaderStatus ACL_FAIL = new EventMeshHeaderStatus(2, "aclFail");
        public static EventMeshHeaderStatus TPS_OVERLOAD = new EventMeshHeaderStatus(3, "tpsOverload");

        private EventMeshHeaderStatus(int code, string desc)
        {
            Code = code;
            Desc = desc;
        }

        public int Code { get; set; }
        public string Desc { get; set; }

        public void Serialize(EventMeshWriterBufferContext context)
        {
            context.WriteInteger(Code);
            context.WriteString(Desc);
        }

        public static EventMeshHeaderStatus Deserialize(EventMeshReaderBufferContext context)
        {
            var code = context.NextInt();
            var desc = context.NextString();
            return new EventMeshHeaderStatus(code, desc);
        }
    }
}
