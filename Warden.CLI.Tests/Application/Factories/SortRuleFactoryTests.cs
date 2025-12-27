using Xunit;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Domain.Rules;
using Warden.CLI.Application.Factories;

namespace Warden.CLI.Tests.Application.Factories
{
    public class SortRuleFactoryTests
    {
        [Theory]
        [InlineData("year", typeof(YearSortRule))]
        [InlineData("month", typeof(MonthSortRule))]
        [InlineData("size", typeof(SizeSortRule))]
        [InlineData("alphabetical", typeof(AlphabeticalSortRule))]
        [InlineData("extension", typeof(ExtensionSortRule))]
        [InlineData("category", typeof(CategorySortRule))]
        public void Create_ReturnsCorrectRule_ForValidInput(string input, Type expectedType)
        {
            var rule = SortRuleFactory.Create(input);

            Assert.NotNull(rule);
            Assert.IsType(expectedType, rule);
        }

        [Fact]
        public void Create_ReturnsNull_ForInvalidInput()
        {
            var rule = SortRuleFactory.Create("invalid input");

            Assert.Null(rule);
        }
    }
}