using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Domain.Enums;
using Warden.CLI.Application.Factories;

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

        public ExitCode ProcessDirectory(string sourceDirectory, bool isDryRun, string[] orderBy)
        {
            try
            {
                var rules = SortRuleFactory.CreateRules(orderBy);
                
                OrganizeReport result = _organizerService.OrganizeDirectory(sourceDirectory, isDryRun, rules);
                
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

        public void ProcessSingleFile(FileInfo file, string sourceDirectory, string[] orderBy)
        {
            var rules = SortRuleFactory.CreateRules(orderBy);

            var fileRecord = _organizerService.OrganizeFile(file, sourceDirectory, false, rules);

            if (fileRecord.Success)
            {
                LogEntry log = new LogEntry
                {
                    TimeStamp = DateTime.Now,
                    BatchId = Guid.NewGuid(),
                    FileName = fileRecord.FileName,
                    SourcePath = fileRecord.SourcePath,
                    DestinationPath = fileRecord.DestinationPath,
                    RuleApplied = string.Join(", ", orderBy),
                    Action = fileRecord.Action,
                };

                _auditSerivce.AddEntry(log);
            }

            _consoleFormatter.RenderSingleEvent(fileRecord);
        }
    }
}