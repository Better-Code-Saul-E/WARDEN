using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class YearSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            if (file.LastWriteTime.Year < 1700 || file.LastWriteTime > DateTime.Now)
            {
                return "_Unknown";
            }

            return file.LastWriteTime.Year.ToString();
        }
    }
}