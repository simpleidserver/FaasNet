using Saunter.Attributes;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.Tests.Bus
{
    public class Streetlight
    {
        /// <summary>
        /// Id of the streetlight.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lat-Long coordinates of the streetlight.
        /// </summary>
        public double[] Position { get; set; }

        /// <summary>
        /// History of light intensity measurements
        /// </summary>
        public List<KeyValuePair<DateTime, int>> LightIntensity { get; set; }
    }

    public class LightMeasuredEvent
    {
        /// <summary>
        /// Id of the streetlight.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Light intensity measured in lumens.
        /// </summary>
        public int Lumens { get; set; }
        /// <summary>
        /// Light intensity measured in lumens.
        /// </summary>
        public DateTime SentAt { get; set; }
    }

    [AsyncApi]
    public class StreetlightMessageBus
    {
        [Channel("publish/light/measured",
            BindingsRef = "publishLightAmqpChannel", 
            Servers = new string[] { "rabbitmq" })]
        [PublishOperation(typeof(LightMeasuredEvent),
            Summary = "Inform about environmental lighting conditions for a particular streetlight.",
            BindingsRef = "publishLightAmqpOperation")] // A simple Publish operation.
        public void PublishLightMeasuredEvent(Streetlight streetlight, int lumens) 
        { 
        
        }
    }
}
