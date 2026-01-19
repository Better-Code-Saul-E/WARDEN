using System.Globalization;
using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class MonthSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            return file.LastWriteTime.ToString("MM-MMMM", CultureInfo.InvariantCulture);
        }
    }
}