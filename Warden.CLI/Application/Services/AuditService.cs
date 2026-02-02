using System.Text.Json;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly string basePath;
        private readonly string logDirectory;
        private string _logFilePath;
        public AuditService()
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            logDirectory = Path.Combine(basePath, ".warden");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, "warden_log.json");
        }
        public void AddEntry(LogEntry entry)
        {
            string serializedLog = JsonSerializer.Serialize(entry);
            File.AppendAllText(_logFilePath, serializedLog + Environment.NewLine);
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
    }
}