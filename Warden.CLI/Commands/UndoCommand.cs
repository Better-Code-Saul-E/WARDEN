using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;

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
                return 0;
            }

            _consoleFormatter.RenderInfo($"Found {lastBatch.Count} files from the last run. Undoing...");

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
                    }
                    catch (Exception ex)
                    {
                        _consoleFormatter.RenderError($"restoring '{entry.FileName}'", ex.Message);
                    }
                }
                else
                {
                    _consoleFormatter.RenderWarning($"skipping '{entry.FileName}'", $"No longer exists at '{entry.DestinationPath}'");
                }
            }

            _consoleFormatter.RenderSuccess("Undo completed successfully!");
            return 0;
        }
    }
}