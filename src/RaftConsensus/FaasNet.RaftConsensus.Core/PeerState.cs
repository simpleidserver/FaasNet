﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FaasNet.RaftConsensus.Core
{
    public record PeerState
    {
        private static object _obj = new object();
        private long _currentTerm = 1;
        private long _previousTerm = 1;
        private string _votedFor;
        private long _commitIndex;
        private long _lastApplied = 0;
        private const string _fileName = "peerstate.info";
        private static Dictionary<string, PeerState> _instances = new Dictionary<string, PeerState>();

        public PeerState()  { }

        private string Directory { get; set; }

        /// <summary>
        /// Previous term.
        /// </summary>
        public long PreviousTerm
        {
            get
            {
                return _previousTerm;
            }
            set
            {
                if (_previousTerm == value) return;
                _previousTerm = value;
                Update();
            }
        }

        /// <summary>
        /// Latest term server has seen (initialized to 0, increases monotonically).
        /// </summary>
        public long CurrentTerm
        {
            get
            {
                return _currentTerm;
            }
            set
            {
                if (_currentTerm == value) return;
                _previousTerm = value;
                _currentTerm = value;
                Update();
            }
        }
        /// <summary>
        /// CandidateId that received vote in current term (or null if none).
        /// </summary>
        public string VotedFor
        {
            get
            {
                return _votedFor;
            }
            set
            {
                if (_votedFor == value) return;
                _votedFor = value;
                Update();
            }
        }
        /// <summary>
        /// Index of highest log entry known to be committed (initialized to 0, increases monotonically).
        /// </summary>
        public long CommitIndex
        {
            get
            {
                return _commitIndex;
            }
            set
            {
                if (value == _commitIndex) return;
                _commitIndex = value;
                Update();
            }
        }
        /// <summary>
        /// Index of highest log entry applied to state machine (initialized to 0, increases monotonically).
        /// </summary>
        public long LastApplied
        {
            get
            {
                return _lastApplied;
            }
            set
            {
                if (value == _lastApplied) return;
                _lastApplied = value;
                Update();
            }
        }

        public void IncreaseCurrentTerm()
        {
            CurrentTerm++;
        }

        public void IncreaseLastIndex()
        {
            LastApplied++;
        }

        public static PeerState New(string path)
        {
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            path = Path.Combine(path, _fileName);
            if (_instances.ContainsKey(path)) return _instances[path];
            var result = new PeerState();
            result.Directory = path;
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                result.Update();
            }
            else result = JsonSerializer.Deserialize<PeerState>(File.ReadAllText(path));
            _instances.Add(path, result);
            return result;
        }

        private void Update()
        {
            if (string.IsNullOrWhiteSpace(Directory)) return;
            lock(_obj)
            {
                var json = JsonSerializer.Serialize(this);
                File.WriteAllText(Directory, json);
            }
        }
    }
}
