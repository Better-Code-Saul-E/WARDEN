using System.Globalization;
using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class MonthSortRuleTests : IDisposable
    {
        private readonly string _tempFile;

        public MonthSortRuleTests()
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

        private FileInfo GetFileWithMonth(int month)
        {
            var date = new DateTime(DateTime.Now.Year, month, 15);

            File.SetCreationTime(_tempFile, date);
            File.SetLastWriteTime(_tempFile, date);

            return new FileInfo(_tempFile);
        }

        [Theory]
        [InlineData(1, "01-January")]
        [InlineData(9, "09-September")]
        [InlineData(12, "12-December")]
        public void GetSubFolderName_Month_ReturnsCorrectlyFormattedString(int month, string expectedFolder)
        {
            var rule = new MonthSortRule();
            var file = GetFileWithMonth(month);

            var result = rule.GetSubFolderName(file);

            Assert.Equal(expectedFolder, result);
        }

        [Fact]
        public void GetSubFolderName_NullFile_ThrowsArgumentNullException()
        {
            var rule = new MonthSortRule();
            FileInfo nullFile = null;

            Assert.Throws<ArgumentNullException>(() => rule.GetSubFolderName(nullFile!));
        }

        [Fact]
        public void GetSubFolderName_IgnoresSystemCulture_AlwaysReturnsEnglish()
        {
            var originalCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                var rule = new MonthSortRule();
                var file = GetFileWithMonth(10);

                var result = rule.GetSubFolderName(file);

                Assert.Equal("10-October", result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

    }
}