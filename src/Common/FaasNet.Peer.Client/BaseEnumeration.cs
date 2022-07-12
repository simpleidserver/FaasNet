﻿using System.Linq;
using System.Reflection;

namespace FaasNet.Peer.Client
{
    public class BaseEnumeration
    {
        protected BaseEnumeration() { }

        protected BaseEnumeration(int code, string name)
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

        public static bool operator ==(BaseEnumeration a, BaseEnumeration b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(BaseEnumeration a, BaseEnumeration b)
        {
            if ((object)a == null || (object)b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public bool Equals(BaseEnumeration other)
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

            var other = obj as BaseEnumeration;
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

        protected void Init<T>(int code) where T : BaseEnumeration
        {
            var cmdType = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Single(f => f.FieldType == typeof(T) && ((T)f.GetValue(null)).Code == code);
            var name = ((T)cmdType.GetValue(null)).Name;
            Code = code;
            Name = name;
        }
    }
}
