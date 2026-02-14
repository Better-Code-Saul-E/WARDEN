using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Commands
{
    public class UndoCommand : Command<UndoSettings>
    {
        private readonly IAuditService _auditService;
        private readonly IFileOrganizerService _organizerService;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;

        public UndoCommand(IAuditService auditService, IFileSystem fileSystem, IConsole console)
        {
            _auditService = auditService;
            _fileSystem = fileSystem;
            _console = console;
        }

        public override int Execute(CommandContext context, UndoSettings settings, CancellationToken cancellationToken)
        {
            var lastBatch = _auditService.GetLastBatch();

            if (lastBatch.Count == 0)
            {
                _console.WriteLine("No recent operatiosn found to undo");
                return 0;
            }

            _console.WriteLine($"Found {lastBatch.Count} files from the last run. Undoing...");

            foreach (var entry in lastBatch)
            {
                if (_fileSystem.FileExists(entry.DestinationPath))
                {
                    try
                    {
                        var sourceDir = Path.GetDirectoryName(entry.SourcePath);

                        if (!_fileSystem.DirectoryExists(sourceDir))
                        {
                            _fileSystem.CreateDirectory(sourceDir);
                        }

                        _fileSystem.MoveFile(entry.DestinationPath, entry.SourcePath);
                        _console.WriteLine($"Restored: {entry.FileName}");
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine($"[Red]Error restoring {entry.FileName}: {ex.Message}[/]");
                    }
                }
                else
                {
                    _console.WriteLine($"[Yellow]Skipping {entry.FileName}: File no longer exists at {entry.DestinationPath}[/]");
                }
            }

            _console.WriteLine("[Green]Undo Complete![/]");
            return 0;
        }
    }
}