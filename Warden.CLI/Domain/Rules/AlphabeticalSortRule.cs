using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Domain.Rules
{
    public class AlphabeticalSortRule : ISortRule
    {
        public string GetSubFolderName(FileInfo file)
        {
            if(file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(file.Name))
            {
                return "_Symbols";
            }

            var firstCharacter = char.ToUpperInvariant(file.Name[0]);

            if (char.IsDigit(firstCharacter))
            {
                return "0-9";
            }
            
            if (char.IsLetter(firstCharacter))
            {
                return firstCharacter.ToString();
            }

            return "_Symbols";
        }
    }
}