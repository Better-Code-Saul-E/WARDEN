using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class YearSortRuleTests : IDisposable
    {
        private readonly string _tempFile;

        public YearSortRuleTests()
        {
            _tempFile = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        private FileInfo GetFileWithMonth(int year, int month, int day)
        {
            var date = new DateTime(year, month, day);

            File.SetCreationTime(_tempFile, date);
            File.SetLastWriteTime(_tempFile, date);

            return new FileInfo(_tempFile);
        }

        [Fact]
        public void GetSubFolderName_ValidDate_ReturnsYear()
        {
            var rule = new YearSortRule();
            var file = GetFileWithMonth(2025, 2, 5);

            var result = rule.GetSubFolderName(file);

            Assert.Equal("2025", result);
        }

        [Theory]
        [InlineData(3000, 1, 1)]
        [InlineData(1699, 1, 1)]
        public void GetSubFolderName_OutOfBoundsDate_ReturnsUnknown(int year, int month, int day)
        {
            var rule = new YearSortRule();
            var file = GetFileWithMonth(year, month, day);

            var result = rule.GetSubFolderName(file);

            Assert.Equal("_Unknown", result);
        }

        [Fact]
        public void GetSubFolderName_LeapYear_ReturnsCorrectYear()
        {
            var rule = new YearSortRule();
            var file = GetFileWithMonth(2024, 2, 29);

            var result = rule.GetSubFolderName(file);

            Assert.Equal("2024", result);
        }

        [Theory]
        [InlineData(2025, 2, 14, "02-February")]
        [InlineData(2026, 12, 24, "12-December")]
        [InlineData(2027, 1, 1, "01-January")]
        public void GetSubFolderName_DifferentMonths_ReturnsFormattedString(int year, int month, int day, string expectedFormatedDate)
        {
            var rule = new MonthSortRule();
            var file = GetFileWithMonth(year, month, day);

            var result = rule.GetSubFolderName(file);

            Assert.Equal(expectedFormatedDate, result);
        }
    }
}