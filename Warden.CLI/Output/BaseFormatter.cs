using Spectre.Console;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public abstract class BaseFormatter
    {
        protected readonly IConsole _console;

        protected BaseFormatter(IConsole console)
        {
            _console = console;
        }

        public void RenderInfo(string message)
        {
            _console.WriteLine(message);
        }
        public void RenderError(string action, string message)
        {
            RenderInfo($"[red]Error[/] {action}: \"{message}\"");
        }
        public void RenderWarning(string action, string message)
        {
            RenderInfo($"[darkorange]Warning[/] {action}: {message}");
        }
        public void RenderSuccess(string message)
        {
            _console.WriteLine($"[green]{message}[/]");
        }
        protected static string FormatFilePath(string path)
        {
            return $"[cyan]{path}[/]";
        }
        protected static string FormatAction(string action)
        {
            if (action.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return $"[red]{action}[/]";
            }
            if (action.StartsWith("Will", StringComparison.OrdinalIgnoreCase))
            {
                return $"[darkorange]{action}[/]";
            }

            return $"[green]{action}[/]";
        }
    }
}