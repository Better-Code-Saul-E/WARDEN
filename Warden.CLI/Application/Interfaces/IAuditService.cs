using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IAuditService
    {
        void AddFromRecord(FileRecord record, Guid batchId, string[] rulesApplied);
        List<LogEntry> GetRecentLogs(int amount);
        List<LogEntry> GetLastBatch();
        void EnforceBatchLimit();
    }
}