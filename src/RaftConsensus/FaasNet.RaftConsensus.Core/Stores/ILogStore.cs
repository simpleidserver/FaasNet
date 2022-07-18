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
        string TermId { get; set; }
        Task<LogRecord> Get(long index, CancellationToken cancellationToken);
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

        public Task<LogRecord> Get(long index, CancellationToken cancellationToken)
        {
            return Task.FromResult(_logRecords.FirstOrDefault(lr => lr.Index == index));
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

        public string TermId { get; set; }

        public void Add(LogRecord logRecord)
        {
            lock(_obj) System.IO.File.AppendAllLines(GetLogPath(), new string[] { logRecord.Value });
        }

        public Task<LogRecord> Get(long index, CancellationToken cancellationToken)
        {
            lock(_obj)
            {
                index = index - 1;
                LogRecord result = null;
                using (var streamReader = new StreamReader(GetLogPath()))
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
                var lines = System.IO.File.ReadAllLines(GetLogPath());
                var result = new List<LogRecord>();
                for (var i = 0; i < lines.Length; i++) result.Add(new LogRecord { Index = i, Value = lines.ElementAt(i) });
                return Task.FromResult(result as IEnumerable<LogRecord>);
            }
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        private string GetLogPath()
        {
            return string.Format(_options.LogFilePath, TermId);
        }
    }


    [DebuggerDisplay("Value = {Value}, Index = {Index}")]
    public class LogRecord
    {
        public long Index { get; set; }
        public string Value { get; set; }
        public DateTime InsertionDateTime { get; set; }
    }
}
