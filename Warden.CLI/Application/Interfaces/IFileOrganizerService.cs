using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileOrganizerService 
    {
        OrganizeReport Organize(string targetPath, bool IsDryRun, string[] orderBy);
        FileRecord ProcessFile(FileInfo file, string rootPath, bool IsDryRun, List<ISortRule> rules);
    }
} 