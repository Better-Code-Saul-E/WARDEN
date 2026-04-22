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

        public void AddFromRecord(FileRecord record, Guid batchId, string[] rulesApplied)
        {
            LogEntry log = new LogEntry
            {
                TimeStamp = DateTime.Now,
                BatchId = batchId,
                FileName = record.FileName,
                SourcePath = record.SourcePath,
                DestinationPath = record.DestinationPath,
                RuleApplied = string.Join(", ", rulesApplied),
                Action = record.Action,
            };

            AddEntry(log);
        }
        public List<LogEntry> GetRecentLogs(int amount)
        {
            List<LogEntry> entries = new List<LogEntry>();

            if (!File.Exists(_logFilePath))
            {
                return entries;
            }

            foreach (var line in File.ReadLines(_logFilePath).Reverse())
            {
                if (entries.Count >= amount) { break; }

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

        private void AddEntry(LogEntry entry)
        {
            string serializedLog = JsonSerializer.Serialize(entry);
            File.AppendAllText(_logFilePath, serializedLog + Environment.NewLine);
        }
    }
}