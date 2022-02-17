using System;

namespace EventMesh.Runtime.Messages
{
    public class UserAgentPurpose : IEquatable<UserAgentPurpose>
    {
        public static UserAgentPurpose SUB = new UserAgentPurpose(1);
        public static UserAgentPurpose PUB = new UserAgentPurpose(2);

        public UserAgentPurpose(int code)
        {
            Code = code;
        }

        public int Code { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Code);
        }

        public static bool operator ==(UserAgentPurpose a, UserAgentPurpose b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(UserAgentPurpose a, UserAgentPurpose b)
        {
            if ((object)a == null || (object)b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public static UserAgentPurpose Deserialize(ReadBufferContext context)
        {
            return new UserAgentPurpose(context.NextInt());
        }

        public bool Equals(UserAgentPurpose other)
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

            var other = obj as UserAgentPurpose;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Code;
        }
    }
}
