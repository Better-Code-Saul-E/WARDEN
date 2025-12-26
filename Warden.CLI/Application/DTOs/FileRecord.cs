namespace Warden.CLI.Application.DTOs
{    
    /// <summary>
    /// Represents the result of processing a single file during the organize operation.
    /// 
    /// It is used to pass file level data 
    /// </summary>
    public class FileRecord
    {
        public string FileName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}