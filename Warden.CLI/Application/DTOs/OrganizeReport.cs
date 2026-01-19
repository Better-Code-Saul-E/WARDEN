namespace Warden.CLI.Application.DTOs
{
    /// <summary>
    /// Represents the aggregated result of a organize operation
    /// </summary>
    public class OrganizeReport
    {
        public bool IsDryRun { get; set; }
        public List<FileRecord> Files { get; set; } = new();
    }
}