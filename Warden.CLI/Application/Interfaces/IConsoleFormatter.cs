using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IConsoleFormatter
    {
        void Render(OrganizeReport result);
        void RenderSingleEvent(FileRecord file);

        void RenderTitle(string title, string path);
        void RenderInstruction(string action, string key, string context);

        void RenderError(string action, string message);
        void RenderWarning(string action, string message);
        void RenderSuccess(string message);
        void RenderInfo(string message);
    }
}