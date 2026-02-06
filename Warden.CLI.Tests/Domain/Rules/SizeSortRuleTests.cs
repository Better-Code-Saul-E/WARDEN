using System.Data;
using Warden.CLI.Domain.Rules;
using Xunit;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class SizeSortRuleTests : IDisposable
    {
        private readonly List<string> _tempFiles = new();

        private FileInfo CreateSizedFile(long bytes)
        {
            var path = Path.GetTempFileName();

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.SetLength(bytes);
            }

            _tempFiles.Add(path);
            return new FileInfo(path);

        }
        
        public void Dispose()
        {
            foreach (var f in _tempFiles)
            {
                if (File.Exists(f))
                {
                    File.Delete(f);
                }
            }
        }

        [Fact]
        public void GetSubFolderName_SmallFile_ReturnsSmall()
        {
            var rule = new SizeSortRule();
            var file = CreateSizedFile(0);
            var result = rule.GetSubFolderName(file);

            Assert.Equal("Small (<1MB)", result);
        }

        [Fact]
        public void GetSubFolderName_MediumFile_ReturnsMedium()
        {
            var rule = new SizeSortRule();
            var file = CreateSizedFile(1024L * 1024L * 50L);
            var result = rule.GetSubFolderName(file);

            Assert.Equal("Medium (1MB-100MB)", result);
        }

        [Fact]
        public void GetSubFolderName_LargeFile_ReturnsLarge()
        {
            var rule = new SizeSortRule();
            var file = CreateSizedFile(1024L * 1024L * 500L);
            var result = rule.GetSubFolderName(file);

            Assert.Equal("Large (100MB-1GB)", result);
        }

        [Fact]
        public void GetSubFolderName_HugeFile_ReturnsHuge()
        {
            var rule = new SizeSortRule();
            var file = CreateSizedFile(1024L * 1024L * 1024L);
            var result = rule.GetSubFolderName(file);

            Assert.Equal("Huge (>1GB)", result);
        }
    }
}