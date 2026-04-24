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
        public void Create_ValidRuleName_ReturnsCorrectRuleType(string input, Type expectedType)
        {
            var rule = SortRuleFactory.Create(input);

            Assert.NotNull(rule);
            Assert.IsType(expectedType, rule);
        }

        [Theory]
        [InlineData("yEar", typeof(YearSortRule))]
        [InlineData("moNtH", typeof(MonthSortRule))]
        [InlineData("SIZE", typeof(SizeSortRule))]
        [InlineData("aLPhabeTIcal", typeof(AlphabeticalSortRule))]
        [InlineData("exTENSion", typeof(ExtensionSortRule))]
        [InlineData("cateGORY", typeof(CategorySortRule))]
        public void Create_MixedCaseRuleName_ReturnsCorrectRuleType(string input, Type expectedType)
        {
            var rule = SortRuleFactory.Create(input);

            Assert.NotNull(rule);
            Assert.IsType(expectedType, rule);
        }

        [Theory]
        [InlineData("date created")]
        [InlineData("category type")]
        [InlineData("file size")]
        public void Create_InvalidRuleName_ReturnsNull(string input)
        {
            var rule = SortRuleFactory.Create(input);
            Assert.Null(rule);
        }

        [Theory]
        [InlineData(new string[] {"year", "month"}, new Type[] { typeof(YearSortRule), typeof(MonthSortRule) }, 2)]
        [InlineData(new string[] {"category", "size"}, new Type[] { typeof(CategorySortRule), typeof(SizeSortRule) }, 2)]
        [InlineData(new string[] {"extension", "alphabetical"}, new Type[] { typeof(ExtensionSortRule), typeof(AlphabeticalSortRule) }, 2)]
        public void CreateRules_ValidRuleNames_ReturnsRulesInOrder(string[] input, Type[] expectedTypes, int expectedlength)
        {
            var rules = SortRuleFactory.CreateRules(input);

            Assert.Equal(expectedlength, rules.Count);

            Assert.IsType(expectedTypes[0], rules[0]);
            Assert.IsType(expectedTypes[1], rules[1]);
        }

        [Fact]
        public void CreateRules_InvalidRuleName_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => SortRuleFactory.CreateRules(new string[] {"INVALID", "category"}));

            Assert.Equal("Unrecognized sort rule 'INVALID'. Valid rules are: year, category, extension, month, size, alphabetical.",exception.Message);
        }   

        [Fact]
        public void CreateRules_EmptyArray_ReturnsCategoryRuleAsFallback()
        {
            var rules = SortRuleFactory.CreateRules(new string[] {});

            Assert.Equal(1, rules.Count);
            Assert.IsType(typeof(CategorySortRule), rules[0]);
        }   
    }
}