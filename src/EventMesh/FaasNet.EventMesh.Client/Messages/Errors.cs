using FaasNet.RaftConsensus.Client.Messages;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class Errors : IEquatable<Errors>
    {
        public static Errors NO_ACTIVE_SESSION = new Errors("NO_ACTIVE_SESSION");
        public static Errors NOT_AUTHORIZED = new Errors("NOT_AUTHORIZED");
        public static Errors NO_BRIDGE_SERVER = new Errors("NO_BRIDGE_SERVER");
        public static Errors TARGET_NOT_REACHABLE = new Errors("TARGET_NOT_REACHABLE");
        public static Errors INVALID_URL = new Errors("INVALID_URL");
        public static Errors INVALID_CLIENT = new Errors("INVALID_CLIENT");
        public static Errors INVALID_SESSION = new Errors("INVALID_SESSION");
        public static Errors INTERNAL_ERROR = new Errors("INTERNAL_ERROR");
        public static Errors INVALID_SEQ = new Errors("INVALID_SEQ");
        public static Errors INVALID_BRIDGE = new Errors("INVALID_BRIDGE");
        public static Errors BRIDGE_NOT_ACTIVE = new Errors("BRIDGE_NOT_ACTIVE");
        public static Errors UNKNOWN_BRIDGE = new Errors("UNKNOWN_BRIDGE");
        public static Errors UNKNOWN_VPN = new Errors("UNKNOWN_VPN");
        public static Errors UNKNOWN_SOURCE_VPN = new Errors("UNKNOWN_SOURCE_VPN");
        public static Errors UNKNOWN_TARGET_VPN = new Errors("UNKNOWN_TARGET_VPN");
        public static Errors UNAUTHORIZED_PUBLISH = new Errors("UNAUTHORIZED_PUBLISH");
        public static Errors UNAUTHORIZED_SUBSCRIBE = new Errors("UNAUTHORIZED_SUBSCRIBE");
        public static Errors SESSION_LIFETIME_TOOLONG = new Errors("SESSION_LIFETIME_TOOLONG");
        public static Errors SESSION_LIFETIME_CANNOT_BE_INFINITE = new Errors("SESSION_LIFETIME_CANNOT_BE_INFINITE");
        public static Errors SESSION_LIFETIME_TOOSHORT = new Errors("SESSION_LIFETIME_TOOSHORT");
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
