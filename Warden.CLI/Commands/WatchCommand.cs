using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Enums;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class WatchCommand : AsyncCommand<SortSettings>
    {
        /* 
        * Delay processing a newly detected file, to ensure the OS has 
        * finished writing the file to the disk.
        */
        private const int FileWriteDelayMs = 1000;

        private readonly OrganizeCommandHandler _commandHandler;
        private readonly IConsoleFormatter _consoleFormatter;
        private readonly IFileSystem _fileSystem;
        private readonly IAuditService _auditService;

        public WatchCommand(OrganizeCommandHandler commandHandler, IConsoleFormatter consoleFormatter, IFileSystem fileSystem, IAuditService auditService)
        {
            _commandHandler = commandHandler;
            _consoleFormatter = consoleFormatter;
            _fileSystem = fileSystem;
            _auditService = auditService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            var sourcePath = Path.GetFullPath(settings.SourcePath);

            if (!_fileSystem.DirectoryExists(sourcePath))
            {
                _consoleFormatter.RenderError("validating path", $"Directory '{sourcePath}' not found.");
                return (int)ExitCode.InvalidPath;
            }

            var filesInDirectory = _fileSystem.GetFiles(sourcePath).Length;
            if (filesInDirectory > 0)
            {
                var proceed = _consoleFormatter.RenderConfirm(
                    $"You are about to sort '{sourcePath}'.",
                    "Please ensure all files are SAVED and CLOSED."
                    );

                if (proceed)
                {
                    _consoleFormatter.RenderInfo($"Performing initial cleanup of '{sourcePath}'...");
                    var exitCode = _commandHandler.ProcessDirectory(sourcePath, false, settings.OrderBy);

                    if (exitCode != ExitCode.Success)
                    {
                        return (int)exitCode;
                    }
                }
                else
                {
                    _consoleFormatter.RenderInfo("Skipping initial cleanup. Existing files will remain in place.");
                    return (int)ExitCode.UserCancelled;
                }

            }


            _consoleFormatter.RenderTitle("Warden is watching", sourcePath);
            _consoleFormatter.RenderInstruction("Press", "Ctrl+C", "to stop.");

            var watchSessionBatchId = Guid.NewGuid();

            using var watcher = new FileSystemWatcher(sourcePath);

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            watcher.Created += async (sender, e) =>
            {
                try
                {
                    await Task.Delay(FileWriteDelayMs, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (!_fileSystem.FileExists(e.FullPath))
                {
                    return;
                }

                _consoleFormatter.RenderInfo($"Detected '{e.Name}'");

                try
                {
                    var fileInfo = new FileInfo(e.FullPath);
                    _commandHandler.ProcessSingleFile(fileInfo, sourcePath, settings.OrderBy, watchSessionBatchId);

                }
                catch (IOException)
                {
                    _consoleFormatter.RenderWarning("checking lock", $"File in use: '{e.Name}'");
                }
                catch (Exception ex)
                {
                    _consoleFormatter.RenderError($"processing '{e.Name}'", ex.Message);
                }

            };

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException) { }
            _consoleFormatter.RenderInfo("Stopping watcher...");
            _consoleFormatter.RenderInfo("Cleaning up audit log...");
            _auditService.EnforceBatchLimit();

            return (int)ExitCode.Success;
        }

    }
}