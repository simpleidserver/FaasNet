using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SubscriptionTypes : IEquatable<SubscriptionTypes>
    {
        /// <summary>
        /// SYNC.
        /// </summary>
        public static SubscriptionTypes SYNC = new SubscriptionTypes("SYNC");
        /// <summary>
        /// ASYNC.
        /// </summary>
        public static SubscriptionTypes ASYNC = new SubscriptionTypes("ASYNC");

        private SubscriptionTypes(string type)
        {
            Type = type;
        }

        public string Type { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Type);
        }

        public static bool operator ==(SubscriptionTypes a, SubscriptionTypes b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SubscriptionTypes a, SubscriptionTypes b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public static SubscriptionTypes Deserialize(ReadBufferContext context)
        {
            return new SubscriptionTypes(context.NextString());
        }

        public bool Equals(SubscriptionTypes other)
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

            var other = obj as SubscriptionTypes;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
