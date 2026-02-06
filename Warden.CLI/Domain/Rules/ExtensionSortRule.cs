using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class ExtensionSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            if(file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var extension = file.Extension.TrimStart('.').ToLowerInvariant();

            return string.IsNullOrEmpty(extension) ? "_NoExtension" : extension;
        }
    }
}