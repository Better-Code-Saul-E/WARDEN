using Spectre.Console;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public class ConsoleFormatter : IConsoleFormatter
    {
        private readonly IConsole _console;

        public ConsoleFormatter(IConsole console)
        {
            _console = console;
        }

        public void Render(OrganizeReport result)
        {
            var table = new Table()
            .AddColumn("File")
            .AddColumn("Category")
            .AddColumn("Action");

            table.Border(TableBorder.Rounded);

            foreach (var file in result.Files)
            {
                table.AddRow(
                    file.FileName,
                    $"[blue]{file.Category}[/]",
                    FormatAction(file)
                );
            }

            _console.Write(table);

            if (result.IsAuditMode)
            {
                _console.WriteLine("\n[yellow]Dry run. no files were moved.[/]");
            }
            else
            {
                _console.WriteLine($"\n[green]Done. Processed {result.Files.Count} files.[/]");
            }
        }
        public void RenderError(string message)
        {
            _console.WriteLine($"[red]Error:[/] {message}");
        }
        private static string FormatAction(FileRecord file)
        {
            if (!file.Success)
            {
                return $"[red]{file.Action}[/]";
            }
            else
            {
                return $"[green]{file.Action}[/]";
            }
        }
    }
}