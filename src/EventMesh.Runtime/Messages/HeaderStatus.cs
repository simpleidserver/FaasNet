namespace EventMesh.Runtime.Messages
{
    public class HeaderStatus
    {
        public static HeaderStatus SUCCESS = new HeaderStatus(0, "success");
        public static HeaderStatus FAIL = new HeaderStatus(1, "fail");
        public static HeaderStatus ACL_FAIL = new HeaderStatus(2, "aclFail");
        public static HeaderStatus TPS_OVERLOAD = new HeaderStatus(3, "tpsOverload");

        private HeaderStatus(int code, string desc)
        {
            Code = code;
            Desc = desc;
        }

        public int Code { get; set; }
        public string Desc { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Code);
            context.WriteString(Desc);
        }

        public static HeaderStatus Deserialize(ReadBufferContext context)
        {
            var code = context.NextInt();
            var desc = context.NextString();
            return new HeaderStatus(code, desc);
        }
    }
}
