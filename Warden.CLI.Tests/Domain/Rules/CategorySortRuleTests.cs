using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class CategorySortRuleTests : IDisposable
    {
        private readonly List<string> _tempFiles = new();

        private FileInfo CreateTempFile(string extension)
        {
            var tempPath = Path.GetTempFileName();
            var newPath = Path.ChangeExtension(tempPath, extension);

            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.Move(tempPath, newPath);

            _tempFiles.Add(newPath);
            return new FileInfo(newPath);
        }
        public void Dispose()
        {
            foreach (var path in _tempFiles)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Theory]
        [InlineData(".mp3", "Audio")]
        [InlineData(".wav", "Audio")]
        [InlineData(".jpg", "Images")]
        [InlineData(".png", "Images")]
        [InlineData(".mp4", "Video")]
        [InlineData(".mkv", "Video")]
        [InlineData(".pdf", "Documents")]
        [InlineData(".docx", "Documents")]
        [InlineData(".exe", "Executables")]
        [InlineData(".xyz", "_Other")]
        public void GetSubFolderName_ReturnsCorrectCategory(string extension, string expectedCategory)
        {
            var rule = new CategorySortRule();
            var file = CreateTempFile(extension);

            var result = rule.GetSubFolderName(file);

            Assert.Equal(expectedCategory, result);

        }
    }
}