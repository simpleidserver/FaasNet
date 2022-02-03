using System;

namespace EventMesh.Runtime.Messages
{
    public class SubscriptionModes : IEquatable<SubscriptionModes>
    {
        /// <summary>
        /// Broadcast.
        /// </summary>
        public static SubscriptionModes BROADCASTING = new SubscriptionModes("BROADCASTING");
        /// <summary>
        /// Clustering.
        /// </summary>
        public static SubscriptionModes CLUSTERING = new SubscriptionModes("CLUSTERING");

        public SubscriptionModes(string mode)
        {
            Mode = mode;
        }

        public string Mode { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Mode);
        }

        public static bool operator ==(SubscriptionModes a, SubscriptionModes b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SubscriptionModes a, SubscriptionModes b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public static SubscriptionModes Deserialize(ReadBufferContext context)
        {
            return new SubscriptionModes(context.NextString());
        }

        public bool Equals(SubscriptionModes other)
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

            var other = obj as SubscriptionModes;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Mode.GetHashCode();
        }
    }
}
