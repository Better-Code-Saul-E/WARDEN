using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class YearSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            if (file.LastWriteTime > DateTime.Now)
            {
                throw new InvalidOperationException("File is from the future!");
            }

            if (file.LastWriteTime.Year < 1700)
            {
                throw new ArgumentOutOfRangeException("File is from the past!");
            }

            return file.LastWriteTime.Year.ToString();
        }
    }
}