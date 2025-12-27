using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class TimeSortRuleTests : IDisposable
    {
        private readonly string _tempFile;
        public TimeSortRuleTests()
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
        private FileInfo GetFileWithDate(int year, int month, int day)
        {
            var date = new DateTime(year, month, day);

            File.SetCreationTime(_tempFile, date);
            File.SetLastWriteTime(_tempFile, date);

            return new FileInfo(_tempFile);
        }

        [Fact]
        public void YearRule_IdentifiesAncientFiles()
        {
            var rule = new YearSortRule();
            var file = GetFileWithDate(1990, 6, 7);

            Assert.Equal("1990", rule.GetSubFolderName(file));
        }

        [Fact]
        public void YearRule_IdentifiesFutureFiles()
        {
            var rule = new YearSortRule();
            var file = GetFileWithDate(2077, 1, 1);

            Assert.Equal("2077", rule.GetSubFolderName(file));
        }

        [Fact]
        public void MonthRule_FormatsWithLeadingZero()
        {
            var rule = new MonthSortRule();
            var file = GetFileWithDate(2023, 2, 14); 

            Assert.Equal("02-February", rule.GetSubFolderName(file));
        }

        [Fact]
        public void MonthRule_HandlesDecemberCorrectly()
        {
            var rule = new MonthSortRule();
            var file = GetFileWithDate(2023, 12, 25);

            Assert.Equal("12-December", rule.GetSubFolderName(file));
        }
    }
}