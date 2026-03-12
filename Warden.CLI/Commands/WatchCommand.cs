using Spectre.Console;
using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Factories;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class WatchCommand : AsyncCommand<SortSettings>
    {
        private readonly OrganizeCommandHandler _commandHandler;

        public WatchCommand(OrganizeCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;

        }
        public override async Task<int> ExecuteAsync(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            var sourcePath = Path.GetFullPath(settings.SourcePath);

            if (!Directory.Exists(sourcePath))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Directory '{sourcePath}' not found.");
                return 1;
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Warden is watching:[/] [blue]{sourcePath}[/]");
            AnsiConsole.MarkupLine("Press [yellow]Ctrl+C[/] to stop.");
            AnsiConsole.WriteLine();

            using var watcher = new FileSystemWatcher(sourcePath);

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            watcher.Created += async (sender, e) =>
            {
                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (!File.Exists(e.FullPath))
                {
                    return;
                }

                AnsiConsole.MarkupLine($"[grey]Detected:[/] {e.Name}");

                try
                {
                    var fileInfo = new FileInfo(e.FullPath);
                    _commandHandler.ProcessSingleFile(fileInfo, sourcePath, settings.OrderBy);

                }
                catch (IOException)
                {
                    AnsiConsole.MarkupLine($"[yellow]Skipped (File in use):[/] {e.Name}");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error processing file:[/] {ex.Message}");
                }

            };

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException) { }

            return 0;
        }

    }
}