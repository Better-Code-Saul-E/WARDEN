using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class SizeSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            var len = file.Length;

            if(len < 1_000_000) return "Small (<1MB)";
            if(len < 1_000_000) return "Medium (<1MB-100MB)";
            if(len < 1_000_000) return "Large (<100MB-1GB)";

            return "Huge (>1GB)";
        }
    }
}