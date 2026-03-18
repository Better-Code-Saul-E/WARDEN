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
                RenderInfo("No audit logs found.");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title($"Audit Log (Last {logs.Count})");

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
                    FormatAction(log.Action),
                    log.FileName,
                    FormatFilePath(log.SourcePath),
                    FormatFilePath(log.DestinationPath)
                );
            }

            _console.Write(table);
        }
        public void RenderError(string action, string message)
        {
            _console.WriteLine($"[red]Error[/] {action}: \"{message}\"");
        }
        public void RenderInfo(string message)
        {
            _console.WriteLine(message);
        }

        private static string FormatFilePath(string path)
        {
            return $"[cyan]{path}[/]";
        }
        private static string FormatAction(string action)
        {
            if (action.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return $"[red]{action}[/]";
            }
            if (action.StartsWith("Will", StringComparison.OrdinalIgnoreCase))
            {
                return $"[darkorange]{action}[/]";
            }

            return $"[green]{action}[/]";
        }
    }
}