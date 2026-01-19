using Spectre.Console.Cli;
using Warden.CLI.Domain.Enums;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class ProbeCommand : Command<SortSettings>
    {
        private readonly OrganizeCommandHandler _commandHandler;

        public ProbeCommand(OrganizeCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public override int Execute(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                var exitCode = _commandHandler.ProcessRequest(settings.SourcePath, true, settings.OrderBy);
                return (int)exitCode;
            }
            catch
            {
                return (int)ExitCode.UnhandledError;
            }
        }
    }
}