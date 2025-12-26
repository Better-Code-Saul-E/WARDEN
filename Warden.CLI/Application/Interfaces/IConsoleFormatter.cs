using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IConsoleFormatter
    {
        void Render(OrganizeReport result);
        void RenderError(string message);
    }
}