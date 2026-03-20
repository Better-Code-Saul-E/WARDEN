using Spectre.Console;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public class AuditFormatter : BaseFormatter, IAuditFormatter
    {
        public AuditFormatter(IConsole console) : base(console)
        {
        }

        public void RenderTable(List<LogEntry> logs)
        {
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
    }
}