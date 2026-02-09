using Spectre.Console;
using Spectre.Console.Cli;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Enums;

namespace Warden.CLI.Commands
{
    public class AuditCommand : Command<AuditSettings>
    {
        private readonly IAuditService _auditService;
        private readonly IAuditFormatter _formatter;

        public AuditCommand(IAuditService auditService, IAuditFormatter formatter)
        {
            _auditService = auditService;
            _formatter = formatter;
        }

        public override int Execute(CommandContext context, AuditSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                List<LogEntry> logEntries = _auditService.GetRecentLogs(settings.Limit);

                if (logEntries.Count == 0)
                {
                    _formatter.RenderError("[yellow]No audit logs found.[/]");
                    return (int)ExitCode.Success;
                }

                _formatter.RenderTable(logEntries);

                return (int)ExitCode.Success;
            }
            catch
            {
                return (int)ExitCode.UnhandledError;
            }
        }
    }
}
