using Spectre.Console.Cli;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Enums;
using Warden.CLI.Handlers;

namespace Warden.CLI.Commands
{
    public class ProbeCommand : Command<SortSettings>
    {
        private readonly OrganizeCommandHandler _commandHandler;
        private readonly IConsoleFormatter _consoleFormatter;

        public ProbeCommand(OrganizeCommandHandler commandHandler, IConsoleFormatter consoleFormatter)
        {
            _commandHandler = commandHandler;
            _consoleFormatter = consoleFormatter;
        }

        public override int Execute(CommandContext context, SortSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                var exitCode = _commandHandler.ProcessDirectory(settings.SourcePath, true, settings.OrderBy);
                return (int)exitCode;
            }
            catch (Exception ex)
            {
                _consoleFormatter.RenderError("executing command", ex.Message);
                return (int)ExitCode.UnhandledError;
            }
        }
    }
}