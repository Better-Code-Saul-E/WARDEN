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
            .AddColumn("Current")
            .AddColumn("Action")
            .AddColumn("Destination");

            table.Border(TableBorder.Rounded);

            foreach (var file in result.Files)
            {
                table.AddRow(
                    file.FileName,
                    FormatFilePath(file.SourcePath),
                    FormatAction(file),
                    FormatFilePath(file.DestinationPath)
                );
            }

            _console.Write(table);

            if (result.IsDryRun)
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

        private static string FormatFilePath(string path)
        {
            return $"[blue]{path}[/]";
        }
        private static string FormatAction(FileRecord file)
        {
            if (!file.Success)
            {
                return $"[red]{file.Action}[/]";
            }
            else if (file.Action.StartsWith("Will"))
            {
                return $"[yellow]{file.Action}[/]";
            }

            return $"[green]{file.Action}[/]";
        }
    }
}