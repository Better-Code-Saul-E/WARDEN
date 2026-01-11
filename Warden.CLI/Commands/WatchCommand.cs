using Spectre.Console;
using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Factories;

namespace Warden.CLI.Commands
{
    public class WatchCommand : AsyncCommand<SortSettings>
    {
        private readonly IFileOrganizerService _organizerService;

        public WatchCommand(IFileOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }
        public override async Task<int> ExecuteAsync(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            var targetPath = Path.GetFullPath(settings.TargetPath);

            if (!Directory.Exists(targetPath))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Directory '{targetPath}' not found.");
                return 1;
            }

            var rules = new List<ISortRule>();
            foreach (var ruleName in settings.OrderBy)
            {
                var rule = SortRuleFactory.Create(ruleName);
                if (rule != null)
                {
                    rules.Add(rule);
                }
            }

            if (!rules.Any())
            {
                rules.Add(SortRuleFactory.Create("extension")!);
            }

            AnsiConsole.MarkupLine($"[yellow]Performing initial cleanup of {targetPath}...[/]");
            var initialReport = _organizerService.Organize(targetPath, false, settings.OrderBy);
            AnsiConsole.MarkupLine($"[green]   Processed {initialReport.Files.Count} existing files.[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine($"[green]Warden is watching:[/] [blue]{targetPath}[/]");
            AnsiConsole.MarkupLine("Press [yellow]Ctrl+C[/] to stop.");
            AnsiConsole.WriteLine();

            using var watcher = new FileSystemWatcher(targetPath);

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
                    var result = _organizerService.ProcessFile(fileInfo, targetPath, false, rules);

                    if (result.Success)
                    {
                        AnsiConsole.MarkupLine($"   [green]-> {result.Action}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"   [red]X {result.Action}[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"   [red]Error processing file:[/] {ex.Message}");
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