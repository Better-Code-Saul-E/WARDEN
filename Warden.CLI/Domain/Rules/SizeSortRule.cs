using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class SizeSortRule : ISortRule
    {
        private const long OneMB = 1024 * 1024;
        private const long OneHundredMB = 100 * OneMB;
        private const long OneGB = 1024 * OneMB;
        public string GetSubFolderName(FileInfo file)
        {
            var len = file.Length;

            if(len < OneMB) return "Small (<1MB)";
            if(len < OneHundredMB) return "Medium (1MB-100MB)";
            if(len < OneGB) return "Large (100MB-1GB)";

            return "Huge (>1GB)";
        }
    }
}