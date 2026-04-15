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
        public static List<ISortRule> CreateRules(string[] ruleNames)
        {
            var rules = new List<ISortRule>();

            foreach (var ruleName in ruleNames)
            {
                var rule = Create(ruleName);

                if (rule == null)
                {
                    throw new ArgumentException($"Unrecognized sort rule '{ruleName}'. Valid rules are: year, category, extension, month, size, alphabetical.");
                }
            }

            if (!rules.Any())
            {
                rules.Add(new CategorySortRule());
            }

            return rules;
        }
    }
}