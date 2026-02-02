namespace Warden.CLI.Application.DTOs
{
    public class LogEntry
    {
        public DateTime TimeStamp { get; set; }
        public Guid BatchId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;
        public string RuleApplied { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}