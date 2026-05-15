using System.Text.Json;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Application.Services
{
    public class AuditService : IAuditService
    {
        private string _logFilePath;

        public AuditService(string? basePath = null)
        {
            string root = basePath ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string logDirectory = Path.Combine(root, ".warden");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, "warden_log.json");
        }

        public void AddBatch(List<FileRecord> records, Guid batchId, string[] rulesApplied)
        {
            if (!records.Any()) return;
            var serializedLines = new List<string>();
            var ruleString = string.Join(", ", rulesApplied);
            var timestamp = DateTime.Now;

            foreach (FileRecord record in records)
            {
                LogEntry log = new LogEntry
                {
                    TimeStamp = timestamp,
                    BatchId = batchId,
                    FileName = record.FileName,
                    SourcePath = record.SourcePath,
                    DestinationPath = record.DestinationPath,
                    RuleApplied = ruleString,
                    Action = record.Action,
                };

                serializedLines.Add(JsonSerializer.Serialize(log));
            }

            File.AppendAllLines(_logFilePath, serializedLines);
        }
        public List<LogEntry> GetRecentLogs(int amount)
        {
            List<LogEntry> entries = new List<LogEntry>();

            if (!File.Exists(_logFilePath))
            {
                return entries;
            }

            var lines = File.ReadAllLines(_logFilePath);

            for (int i = 1; i <= lines.Length && entries.Count < amount; i++)
            {
                var line = lines[^i];

                if (string.IsNullOrWhiteSpace(line)) { continue; }

                try
                {
                    var entry = JsonSerializer.Deserialize<LogEntry>(line);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch (JsonException) { }
            }

            return entries;
        }
        public List<LogEntry> GetLastBatch()
        {
            var batch = new List<LogEntry>();

            if (!File.Exists(_logFilePath))
            {
                return batch;
            }

            Guid? currentBatchID = null;

            foreach (var line in File.ReadLines(_logFilePath).Reverse())
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var entry = JsonSerializer.Deserialize<LogEntry>(line);

                    if (entry == null)
                    {
                        continue;
                    }

                    if (currentBatchID == null)
                    {
                        currentBatchID = entry.BatchId;
                    }

                    if (entry.BatchId == currentBatchID)
                    {
                        batch.Add(entry);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (JsonException) { }
            }

            return batch;
        }
        public void EnforceBatchLimit()
        {
            if (!File.Exists(_logFilePath))
            {
                return;
            }

            List<string> entries = new List<string>();
            HashSet<Guid> batches = new HashSet<Guid>();

            foreach (var line in File.ReadLines(_logFilePath).Reverse())
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var entry = JsonSerializer.Deserialize<LogEntry>(line);

                    if (entry == null)
                    {
                        continue;
                    }

                    if (!batches.Contains(entry.BatchId))
                    {
                        if (batches.Count >= 10)
                        {
                            break;
                        }
                        batches.Add(entry.BatchId);
                    }

                    entries.Add(line);
                }
                catch (JsonException) { }
            }

            entries.Reverse();
            File.WriteAllLines(_logFilePath, entries);
        }
    }
}