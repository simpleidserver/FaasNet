using FaasNet.RaftConsensus.Client.Messages;
using System;
using System.Linq;
using System.Reflection;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UserAgentPurpose : IEquatable<UserAgentPurpose>
    {
        public static UserAgentPurpose SUB = new UserAgentPurpose(1, "SUB");
        public static UserAgentPurpose PUB = new UserAgentPurpose(2, "PUB");

        public UserAgentPurpose(int code)
        {
            var cmdType = typeof(UserAgentPurpose).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Single(f => f.FieldType == typeof(UserAgentPurpose) && ((UserAgentPurpose)f.GetValue(null)).Code == code);
            var name = ((UserAgentPurpose)cmdType.GetValue(null)).Name;
            Code = code;
            Name = name;
        }

        public UserAgentPurpose(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public int Code { get; private set; }
        public string Name { get; private set; }

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

        public override string ToString()
        {
            return Name;
        }
    }
}
