using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileOrganizerService 
    {
        OrganizeReport Organize(string sourceDirectory, bool IsDryRun, string[] orderBy);
        FileRecord ProcessFile(FileInfo file, string sourceDirectory, bool IsDryRun, List<ISortRule> rules);
    }
} 