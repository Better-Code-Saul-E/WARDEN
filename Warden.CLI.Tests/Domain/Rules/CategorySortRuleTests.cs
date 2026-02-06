using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class CategorySortRuleTests
    {
        [Theory]
        [InlineData(".jpg", "Images")]
        [InlineData(".PNG", "Images")]
        [InlineData(".pdf", "Documents")]
        [InlineData(".DOCX", "Documents")]
        [InlineData(".mp3", "Audio")]
        [InlineData(".WAV", "Audio")]
        [InlineData(".mp4", "Video")]
        [InlineData(".MKV", "Video")]
        [InlineData(".zip", "Archives")]
        [InlineData(".GZ", "Archives")]
        [InlineData(".exe", "Executables")]
        [InlineData(".ISO", "Executables")]
        [InlineData(".py", "Code")]
        [InlineData(".XML", "Code")]
        [InlineData(".XYZ", "_Other")]
        [InlineData("", "_Other")]
        public void GetSubFolderName_VariousExtensions_ReturnsCorrectCategory(string extension, string expectedCategory)
        {

            var rule = new CategorySortRule();
            var file = new FileInfo($"temp_file{extension}");

            var result = rule.GetSubFolderName(file);

            Assert.Equal(expectedCategory, result);

        }

        [Fact]
        public void GetSubFolderName_NullFile_ThrowsArgumentNullException()
        {
            var rule = new CategorySortRule();
            FileInfo nullFile = null;

            Assert.Throws<ArgumentNullException>(() => rule.GetSubFolderName(nullFile!));
        }
    }
}