using Spectre.Console;
using Spectre.Console.Rendering;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Output
{
    public class SystemConsole : IConsole
    {
        public void Write(IRenderable value)
        {
            AnsiConsole.Write(value);
        }
        public void WriteLine(string text)
        {
            AnsiConsole.MarkupLine(text);
        }
    }
}