using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Enums;

namespace Warden.CLI.Commands
{
    public class UndoCommand : Command<UndoSettings>
    {
        private readonly IAuditService _auditService;
        private readonly IFileSystem _fileSystem;
        private readonly IConsoleFormatter _consoleFormatter;

        public UndoCommand(IAuditService auditService, IFileSystem fileSystem, IConsoleFormatter consoleFormatter)
        {
            _auditService = auditService;
            _fileSystem = fileSystem;
            _consoleFormatter = consoleFormatter;
        }

        public override int Execute(CommandContext context, UndoSettings settings, CancellationToken cancellationToken)
        {
            var lastBatch = _auditService.GetLastBatch();

            if (lastBatch.Count == 0)
            {
                _consoleFormatter.RenderInfo("No recent operations found to undo");
                return (int)ExitCode.NothingToDo;
            }

            if (!settings.Force)
            {
                var missingFiles = lastBatch.Where(entry => !_fileSystem.FileExists(entry.DestinationPath)).ToList();

                if (missingFiles.Any())
                {
                    _consoleFormatter.RenderError(
                        "Undo aborted",
                        $"Found {missingFiles.Count} missing file(s). For example, '{missingFiles.First().FileName}' is no longer at its destination.\n" +
                        "Run 'warden undo --force' to ignore missing files and undo the rest."
                    );
                    return (int)ExitCode.FileNotFound;
                }
            }

            _consoleFormatter.RenderInfo($"Found {lastBatch.Count} files from the last run. Undoing...");

            int restoredCount = 0;
            int failedCount = 0;

            foreach (var entry in lastBatch)
            {
                if (_fileSystem.FileExists(entry.DestinationPath))
                {
                    try
                    {
                        var sourceDir = Path.GetDirectoryName(entry.SourcePath);

                        if (sourceDir != null && !_fileSystem.DirectoryExists(sourceDir))
                        {
                            _fileSystem.CreateDirectory(sourceDir);
                        }

                        _fileSystem.MoveFile(entry.DestinationPath, entry.SourcePath);
                        _consoleFormatter.RenderInfo($"Restored '{entry.FileName}'");
                        restoredCount++;
                    }
                    catch (Exception ex)
                    {
                        _consoleFormatter.RenderError($"attempting to restore '{entry.FileName}'", ex.Message);
                        failedCount++;
                    }
                }
                else
                {
                    _consoleFormatter.RenderWarning($"skipping '{entry.FileName}'", $"No longer exists at '{entry.DestinationPath}'");
                    failedCount++;
                }
            }

            if (failedCount > 0)
            {
                if (restoredCount == 0)
                {
                    _consoleFormatter.RenderError("Undo Failed", "Could not restore any files.");
                    return (int)ExitCode.UnhandledError;
                }
                else
                {
                    _consoleFormatter.RenderWarning("Undo Partially Completed", $"Successfully restored {restoredCount} file(s), but {failedCount} failed.");
                    return (int)ExitCode.PartialSuccess;
                }
            }

            _consoleFormatter.RenderSuccess("Undo completed successfully!");
            return (int)ExitCode.Success;
        }
    }
}