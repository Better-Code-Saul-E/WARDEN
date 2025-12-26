using Spectre.Console.Rendering;

namespace Warden.CLI.Application.Interfaces
{
    public interface IConsole
    {
        void Write(IRenderable value);
        void WriteLine(string text);
    }
}