using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class YearSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            try
            {
                return file.LastWriteTime.Year.ToString();
            }
            catch
            {
                return "Unknown_Date";
            }
        }
    }
}