using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Domain.Enums;

namespace Warden.CLI.Handlers
{
    public class OrganizeCommandHandler
    {
        private readonly IFileOrganizerService _organizerService;
        private readonly IAuditService _auditSerivce;
        private readonly IConsoleFormatter _consoleFormatter;

        public OrganizeCommandHandler(IFileOrganizerService organizerService, IAuditService auditService, IConsoleFormatter consoleFormatter)
        {
            _organizerService = organizerService;
            _auditSerivce = auditService;
            _consoleFormatter = consoleFormatter;
        }

        public ExitCode ProcessRequest(string sourceDirectory, bool isDryRun, string[] orderBy)
        {
            try
            {
                OrganizeReport result = _organizerService.Organize(sourceDirectory, isDryRun, orderBy);
                if (!isDryRun)
                {
                    var BatchId = Guid.NewGuid();

                    foreach (var fileRecord in result.Files)
                    {
                        LogEntry log = new LogEntry
                        {
                            TimeStamp = DateTime.Now,
                            BatchId = BatchId,
                            FileName = fileRecord.FileName,
                            SourcePath = fileRecord.SourcePath,
                            DestinationPath = fileRecord.DestinationPath,
                            RuleApplied = string.Join(", ", orderBy),
                            Action = fileRecord.Action,
                        };
                        _auditSerivce.AddEntry(log);
                    }
                }

                _consoleFormatter.Render(result);
                return ExitCode.Success;
            }
            catch (DirectoryNotFoundException ex)
            {
                _consoleFormatter.RenderError($"Directory not found: {ex.Message}");
                return ExitCode.InvalidPath;
            }
            catch (Exception ex)
            {
                _consoleFormatter.RenderError($"Unexpected error: {ex.Message}");
                return ExitCode.UnhandledError;
            }
        }
    }
}