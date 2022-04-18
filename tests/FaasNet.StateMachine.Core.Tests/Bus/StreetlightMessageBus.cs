using Saunter.Attributes;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.Tests.Bus
{
    public class Streetlight
    {
        public int Id { get; set; }
        public double[] Position { get; set; }
        public List<KeyValuePair<DateTime, int>> LightIntensity { get; set; }
    }

    public class LightMeasuredEvent
    {
        public int Id { get; set; }
        public int Lumens { get; set; }
        public DateTime SentAt { get; set; }
    }

    [AsyncApi]
    public class StreetlightMessageBus
    {
        [Channel("publish/light/measured",
            BindingsRef = "publishLightAmqpChannel",
            Servers = new string[] { "rabbitmq" })]
        [PublishOperation(typeof(LightMeasuredEvent),
            Description = "description",
            Summary = "Inform about environmental lighting conditions for a particular streetlight.",
            BindingsRef = "publishLightAmqpOperation")]
        public void PublishLightMeasuredEvent(Streetlight streetlight, int lumens) 
        { 
        
        }
    }
}
