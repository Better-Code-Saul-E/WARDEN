using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class ExtensionSortRuleTests
    {
        [Theory]
        [InlineData("picture.pdf", "pdf")]
        [InlineData("IMAGE.JPG", "jpg")]
        [InlineData("archive.tar.gz", "gz")]
        public void GetSubFolderName_ValidExtension_ReturnsLowercasedString(string fileName, string expected)
        {
            var rule = new ExtensionSortRule();
            var file = new FileInfo(fileName);

            var result = rule.GetSubFolderName(file);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetSubFolderName_FileWithoutExtension_ReturnsNoExtensionLabel()
        {
            var rule = new ExtensionSortRule();
            var file = new FileInfo("README");

            var result = rule.GetSubFolderName(file);

            Assert.Equal("_NoExtension", result);
        }

        [Fact]
        public void GetSubFolderName_NullFile_ThrowsArgumentNullException()
        {
            var rule = new ExtensionSortRule();
            FileInfo nullFile = null;

            Assert.Throws<ArgumentNullException>(() => rule.GetSubFolderName(nullFile!));
        }
    }
}