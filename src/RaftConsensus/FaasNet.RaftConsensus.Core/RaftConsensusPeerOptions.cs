﻿using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Reflection;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusPeerOptions
    {
        /// <summary>
        /// Random election timer in MS.
        /// </summary>
        public Interval ElectionTimer { get; set; } = new Interval(1000, 10000);
        /// <summary>
        /// Interval in MS used by the leader to send Heartbeat.
        /// </summary>
        public int LeaderHeartbeatTimerMS { get; set; } = 2000;
        /// <summary>s
        /// Sliding expiration time of the heartbeat.
        /// </summary>
        public int LeaderHeartbeatExpirationDurationMS { get; set; } = 12000;
        /// <summary>
        /// Configuration's folder location.
        /// </summary>
        public string ConfigurationDirectoryPath { get; set; }
        /// <summary>
        /// Configuration is store in memory.
        /// </summary>
        public bool IsConfigurationStoredInMemory { get; set; } = false;
        /// <summary>
        /// Action is called when the Peer is a leader.
        /// </summary>
        public Action LeaderCallback { get; set; }
        /// <summary>
        /// Expiration time of a client request.
        /// </summary>
        public int RequestExpirationTimeMS { get; set; } = 5000;
        /// <summary>
        /// Every N logs take a snapshot.
        /// </summary>
        public int SnapshotFrequency { get; set; } = 1;
        /// <summary>
        /// Type of the state machine.
        /// </summary>
        public Type StateMachineType { get; set; } = typeof(GCounter);
    }

    public class Interval
    {
        public Interval(int minMS, int maxMS)
        {
            MinMS = minMS;
            MaxMS = maxMS;
        }

        public int MinMS { get; private set; }
        public int MaxMS { get; private set; }

        public int GetValue()
        {
            return new Random().Next(MinMS, MaxMS);
        }
    }
}
