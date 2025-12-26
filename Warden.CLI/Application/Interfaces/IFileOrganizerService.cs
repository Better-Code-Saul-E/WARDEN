using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileOrganizerService
    {
        OrganizeReport Organize(string targetPath, bool isAuditMode, string[] orderBy);
        FileRecord ProcessFile(FileInfo file, string rootPath, bool isAuditMode, List<ISortRule> rules);
    }
}