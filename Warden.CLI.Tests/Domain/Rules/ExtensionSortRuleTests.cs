using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class ExtensionSortRuleTests : IDisposable
    {
        private string? _tempPath;
        private FileInfo CreateTempFile(string extension)
        {
            var tempPath = Path.GetTempFileName();
            var newPath = Path.ChangeExtension(tempPath, extension);

            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.Move(tempPath, newPath);

            _tempPath = newPath;
            return new FileInfo(newPath);
        }
        public void Dispose()
        {
            if (_tempPath != null && File.Exists(_tempPath))
            {
                File.Delete(_tempPath);
            }
        }
        [Fact]
        public void GetSubFolderName_ReturnsExtensionWithoutDot()
        {
            var rule = new ExtensionSortRule();
            var file = CreateTempFile(".pdf");

            var result = rule.GetSubFolderName(file);

            Assert.Equal("pdf", result);
        }
        [Fact]
        public void GetSubFolderName_ReturnsLowerCasedExtension()
        {
            var rule = new ExtensionSortRule();
            var file = CreateTempFile(".JPG");

            var result = rule.GetSubFolderName(file);

            Assert.Equal("jpg", result);
        }

        [Fact]
        public void GetSubFolderName_ReturnsUnknown_WhenNoExtension()
        {
            var rule = new ExtensionSortRule();
            var file = CreateTempFile("");

            var result = rule.GetSubFolderName(file);

            Assert.Equal("_NoExtension", result);
        }
    }
}