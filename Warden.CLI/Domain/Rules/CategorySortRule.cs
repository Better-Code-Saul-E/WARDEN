using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class CategorySortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            if(file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            
            return FileCategoryRules.GetCategory(file.Extension);
        }
    }
}