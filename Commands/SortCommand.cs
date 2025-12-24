using Spectre.Console.Cli;
using Warden.CLI.Domain.Enums;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class SortCommand : Command<SortSettings>
    {
        private readonly OrganizeCommandHandler _commandHandler;

        public SortCommand(OrganizeCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public override int Execute(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                var exitCode = _commandHandler.ProcessRequest(settings.TargetPath, settings.IsAuditMode, settings.OrderBy);
                return (int)exitCode;
            }
            catch (Exception ex)
            {
                return (int)ExitCode.UnhandledError;
            }
        }
    }
}
