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

        public AuditCommand(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public override int Execute(CommandContext context, AuditSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                List<LogEntry> logEntries = _auditService.GetRecentLogs(settings.Limit);
                if (logEntries.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No audit logs found.[/]");
                    return (int)ExitCode.Success;
                }

                RenderTable(logEntries, settings.Limit);

                return (int)ExitCode.Success;
            }
            catch
            {
                return (int)ExitCode.UnhandledError;
            }
        }

        private void RenderTable(List<LogEntry> logs, int limit)
        {
            var table = new Table();
            table.Border(TableBorder.Rounded); 
            table.Title($"[blue]Audit Log (Last {limit})[/]");

            table.AddColumn("Time");
            table.AddColumn("Action");
            table.AddColumn("File");
            table.AddColumn("From");
            table.AddColumn("To");

            foreach (var log in logs)
            {
                var actionColor = log.Action.Contains("Error") ? "red" : "green";

                table.AddRow(
                    log.TimeStamp.ToString("g"),
                    $"[{actionColor}]{Markup.Escape(log.Action)}[/]", 
                    Markup.Escape(log.FileName),
                    $"[blue]{Markup.Escape(log.SourcePath)}[/]",    
                    $"[blue]{Markup.Escape(log.DestinationPath)}[/]" 
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
