using System;

namespace EventMesh.Runtime.Messages
{
    public class HeaderStatus : IEquatable<HeaderStatus>
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

        public static bool operator ==(HeaderStatus a, HeaderStatus b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(HeaderStatus a, HeaderStatus b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

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

        public bool Equals(HeaderStatus other)
        {
            if (other == null)
            {
                return false;
            }

            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as HeaderStatus;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Code;
        }
    }
}
