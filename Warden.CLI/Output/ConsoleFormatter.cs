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
                RenderWarning("dry run", "No files were moved.");
            }
            else
            {
                RenderSuccess($"Done. Processed {result.Files.Count} files.");
            }
        }
        public void RenderSingleEvent(FileRecord file)
        {
            var action = FormatAction(file);
            var path = FormatFilePath(file.FileName);

            var indent = "\t";

            if (file.Success)
            {
                RenderInfo($"{indent}-> {action}: {path}");
            }
            else
            {
                RenderInfo($"{indent}X  {action}: {path}");
            }
        }
        public void RenderTitle(string title, string path)
        {
            _console.WriteLine($"\n[magenta]{title}:[/] {FormatFilePath(path)}\n");
        }
        public void RenderInstruction(string action, string key, string context)
        {
            _console.WriteLine($"{action} [yellow]{key}[/] [grey]{context}[/]");
        }
        public void RenderError(string action, string message)
        {
            _console.WriteLine($"[red]Error[/] {action}: \"{message}\"");
        }
        public void RenderWarning(string action, string message)
        {
            _console.WriteLine($"[darkorange]Warning[/] {action}: {message}");

        }
        public void RenderSuccess(string message)
        {
            _console.WriteLine($"[green]{message}[/]");
        }
        public void RenderInfo(string message)
        {
            _console.WriteLine(message);
        }

        private static string FormatFilePath(string path)
        {
            return $"[cyan]{path}[/]";
        }
        private static string FormatAction(FileRecord file)
        {
            if (!file.Success)
            {
                return $"[red]{file.Action}[/]";
            }
            else if (file.Action.StartsWith("Will"))
            {
                return $"[darkorange]{file.Action}[/]";
            }

            return $"[green]{file.Action}[/]";
        }
    }
}