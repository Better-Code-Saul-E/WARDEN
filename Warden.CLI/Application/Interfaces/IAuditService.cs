using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IAuditService
    {
        void AddEntry(LogEntry entry);
        List<LogEntry> GetRecentLogs(int amount);
    }
}