using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ILogStore
    {
        Task<LogRecord> Get(string replicationId, long index, CancellationToken cancellationToken);
        Task<LogRecord> GetLatest(string replicationId, CancellationToken cancellationToken);
        void Add(LogRecord logRecord);
    }

    public class InMemoryLogStore : ILogStore
    {
        private readonly ConcurrentBag<LogRecord> _logRecords;

        public InMemoryLogStore()
        {
            _logRecords = new ConcurrentBag<LogRecord>();
        }

        public string TermId { get; set; }

        public void Add(LogRecord logRecord)
        {
            _logRecords.Add(logRecord);
        }

        public Task<LogRecord> Get(string replicationId, long index, CancellationToken cancellationToken)
        {
            return Task.FromResult(_logRecords.FirstOrDefault(lr => lr.ReplicationId == replicationId && lr.Index == index));
        }

        public Task<LogRecord> GetLatest(string replicationId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_logRecords.OrderByDescending(l => l.Index).FirstOrDefault(lr => lr.ReplicationId == replicationId));
        }
    }

    public class FileLogStore : ILogStore
    {
        private readonly FileLogStoreOptions _options;
        private static object _obj = new object();

        public FileLogStore(IOptions<FileLogStoreOptions> options)
        {
            _options = options.Value;
        }

        public void Add(LogRecord logRecord)
        {
            lock(_obj) System.IO.File.AppendAllLines(GetLogFilePath(logRecord.ReplicationId), new string[] { logRecord.Value });
        }

        public Task<LogRecord> Get(string replicationId, long index, CancellationToken cancellationToken)
        {
            lock(_obj)
            {
                index = index - 1;
                LogRecord result = null;
                using (var streamReader = new StreamReader(GetLogFilePath(replicationId)))
                {
                    string line;
                    int currentLine = 0;
                    while((line = streamReader.ReadLine()) != null)
                    {
                        if (index == currentLine) { result = new LogRecord { Value = line, Index = currentLine }; };
                        currentLine++;
                    }
                }

                return Task.FromResult(result);
            }
        }

        public Task<IEnumerable<LogRecord>> GetAll(CancellationToken cancellationToken)
        {
            lock(_obj)
            {
                var result = new List<LogRecord>();
                var directoryInfo = new DirectoryInfo(GetLogPath());
                var allFiles = directoryInfo.GetFiles();
                foreach(var file in allFiles)
                {
                    var lines = File.ReadAllLines(file.FullName);
                    var replicationId = file.Name.Replace(".txt", "");
                    for (var i = 0; i < lines.Length; i++) result.Add(new LogRecord { ReplicationId = replicationId, Index = i, Value = lines.ElementAt(i) });
                }

                return Task.FromResult(result as IEnumerable<LogRecord>);
            }
        }

        public Task<LogRecord> GetLatest(string replicationId, CancellationToken cancellationToken)
        {
            lock (_obj)
            {
                var result = new List<LogRecord>();
                using (var streamReader = new StreamReader(GetLogFilePath(replicationId)))
                {
                    string line = null;
                    int currentLine = 0;
                    while (streamReader.EndOfStream == false)
                    {
                        line = streamReader.ReadLine();
                        currentLine++;
                    }

                    return Task.FromResult(new LogRecord { Value = line, Index = currentLine });
                }
            }
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        private string GetLogFilePath(string replicationId)
        {
            return Path.Combine(GetLogPath(), $"{replicationId}.txt");
        }

        private string GetLogPath()
        {
            if (string.IsNullOrWhiteSpace(_options.LogFilePath)) return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _options.LogFilePath);
        }
    }


    [DebuggerDisplay("Value = {Value}, Index = {Index}, ReplicationId = {ReplicationId}")]
    public class LogRecord
    {
        public string ReplicationId { get; set; }
        public long Index { get; set; }
        public string Value { get; set; }
        public DateTime InsertionDateTime { get; set; }
    }
}
