using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileOrganizerService
    {
        OrganizeReport Organize(string targetPath, bool isAuditMode);
        FileRecord ProcessFile(FileInfo file, string rootPath, bool isAuditMode);
    }
}