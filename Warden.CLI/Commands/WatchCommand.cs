using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class WatchCommand : AsyncCommand<SortSettings>
    {
        private readonly OrganizeCommandHandler _commandHandler;
        private readonly IConsoleFormatter _consoleFormatter;

        public WatchCommand(OrganizeCommandHandler commandHandler, IConsoleFormatter consoleFormatter)
        {
            _commandHandler = commandHandler;
            _consoleFormatter = consoleFormatter;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            var sourcePath = Path.GetFullPath(settings.SourcePath);

            if (!Directory.Exists(sourcePath))
            {
                _consoleFormatter.RenderError($"Directory '{sourcePath}' not found.");
                return 1;
            }

            _consoleFormatter.RenderInfo($"Performing initial cleanup of {sourcePath}...");
            var exitCode = _commandHandler.ProcessRequest(sourcePath, false, settings.OrderBy);

            if (exitCode != Domain.Enums.ExitCode.Success)
            {
                return (int)exitCode;
            }

            _consoleFormatter.RenderTitle("Warden is watching", sourcePath);
            _consoleFormatter.RenderInstruction("Press", "Ctrl+C", "to stop.");

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

                _consoleFormatter.RenderInfo($"Detected '{e.Name}'");

                try
                {
                    var fileInfo = new FileInfo(e.FullPath);
                    _commandHandler.ProcessSingleFile(fileInfo, sourcePath, settings.OrderBy);

                }
                catch (IOException)
                {
                    _consoleFormatter.RenderWarning($"Skipped (File in use) '{e.Name}'");
                }
                catch (Exception ex)
                {
                    _consoleFormatter.RenderError($"Error processing file '{ex.Message}'");
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