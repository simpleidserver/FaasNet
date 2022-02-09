using System;

namespace EventMesh.Runtime.Messages
{
    public class Errors : IEquatable<Errors>
    {
        public static Errors NO_ACTIVE_SESSION = new Errors("NO_ACTIVE_SESSION");
        public static Errors NOT_AUTHORIZED = new Errors("NOT_AUTHORIZED");
        public static Errors INVALID_URL = new Errors("INVALID_URL");
        public static Errors INVALID_CLIENT = new Errors("INVALID_CLIENT");
        public static Errors INVALID_SEQ = new Errors("INVALID_SEQ");
        public static Errors BRIDGE_NOT_ACTIVE = new Errors("BRIDGE_NOT_ACTIVE");
        public static Errors BRIDGE_EXISTS = new Errors("BRIDGE_EXISTS");

        private Errors(string code)
        {
            Code = code;
        }

        #region Properties

        public string Code { get; private set; }

        #endregion

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Code);
        }

        public static Errors Deserialize(ReadBufferContext context)
        {
            return new Errors(context.NextString());
        }

        public static bool operator ==(Errors a, Errors b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Errors a, Errors b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public bool Equals(Errors other)
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

            var other = obj as Commands;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
