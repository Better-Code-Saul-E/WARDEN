using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IAuditFormatter
    {
        void RenderTable(List<LogEntry> logs);
        void RenderError(string message);
    }
}