using FaasNet.RaftConsensus.Client.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SubscriptionRequest : Package
    {
        public SubscriptionRequest()
        {
            TopicFilters = new List<SubscriptionItem>();
        }

        #region Properties

        public string SessionId { get; set; }
        public ICollection<SubscriptionItem> TopicFilters { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(SessionId);
            context.WriteInteger(TopicFilters.Count());
            foreach(var topic in TopicFilters)
            {
                topic.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            SessionId = context.NextString();
            int nbTopics = context.NextInt();
            for(int i = 0; i < nbTopics; i++)
            {
                TopicFilters.Add(SubscriptionItem.Deserialize(context));
            }
        }
    }

    public class SubscriptionItem
    {
        public string Topic { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Topic);
        }

        public static SubscriptionItem Deserialize(ReadBufferContext context)
        {
            return new SubscriptionItem
            {
                Topic = context.NextString()
            };
        }
    }

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
