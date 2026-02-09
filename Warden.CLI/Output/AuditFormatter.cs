using Spectre.Console;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public class AuditFormatter : IAuditFormatter
    {
        private readonly IConsole _console;

        public AuditFormatter(IConsole console)
        {
            _console = console;
        }

        public void RenderTable(List<LogEntry> logs)
        {
            if (logs == null || logs.Count == 0)
            {
                _console.WriteLine("[yellow]No audit logs found.[/]");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title($"[blue]Audit Log (Last {logs.Count})[/]");

            table.AddColumn("Time");
            table.AddColumn("Action");
            table.AddColumn("File");
            table.AddColumn("From");
            table.AddColumn("To");

            foreach (var log in logs)
            {
                var actionColor = log.Action.Contains("Error", StringComparison.OrdinalIgnoreCase) ? "red" : "green";

                table.AddRow(
                    log.TimeStamp.ToString("g"),
                    $"[{actionColor}]{Markup.Escape(log.Action)}[/]",
                    Markup.Escape(log.FileName),
                    $"[blue]{Markup.Escape(log.SourcePath)}[/]",
                    $"[blue]{Markup.Escape(log.DestinationPath)}[/]"
                );
            }

            _console.Write(table);
        }
        public void RenderError(string message)
        {
            _console.WriteLine($"[red]Error:[/] {message}");
        }
    }
}