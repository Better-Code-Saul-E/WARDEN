using Warden.CLI.Application.DTOs;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileOrganizerService 
    {
        OrganizeReport OrganizeDirectory(string sourceDirectory, bool IsDryRun, List<ISortRule> rules);
        FileRecord OrganizeFile(FileInfo file, string sourceDirectory, bool isDryRun, List<ISortRule> rules);
    }
} 