using Spectre.Console;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public class ConsoleFormatter : BaseFormatter, IConsoleFormatter
    {
        public ConsoleFormatter(IConsole console) : base(console)
        {
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
                    FormatAction(file.Action),
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
            var action = FormatAction(file.Action);
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
            RenderInfo($"\n[magenta]{title}:[/] {FormatFilePath(path)}\n");
        }

        public void RenderInstruction(string action, string key, string context)
        {
            RenderInfo($"{action} [yellow]{key}[/] [grey]{context}[/]");
        }
        public bool RenderConfirm(string message, string? warning = null)
        {
            var prompt = message;

            if (!string.IsNullOrEmpty(warning))
            {
                prompt += $"\n[yellow]{warning}[/]";
            }

            prompt += "\nAre you sure you want to continue?";

            return AnsiConsole.Confirm(prompt);
        }
    }
}