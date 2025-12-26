using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Rules;

namespace Warden.CLI.Application.Factories
{
    public static class SortRuleFactory
    {
        public static ISortRule? Create(string ruleName)
        {
            return ruleName?.ToLower() switch
            {
                "year" => new YearSortRule(),
                "category" => new CategorySortRule(),
                "extension" => new ExtensionSortRule(),
                "month" => new MonthSortRule(),
                "size" => new SizeSortRule(),
                "alphabetical" => new AlphabeticalSortRule(),
                _ => null
            };
        }
    }
}