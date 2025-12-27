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
        public void ZeroBytes_ReturnsSmall()
        {
            var file = CreateSizedFile(0); 
            var result = new SizeSortRule().GetSubFolderName(file);
            Assert.Equal("Small (<1MB)", result);
        }

        [Fact]
        public void JustUnderOneMB_ReturnsSmall()
        {
            var file = CreateSizedFile(1024 * 1024 - 1); 
            var result = new SizeSortRule().GetSubFolderName(file);
            Assert.Equal("Small (<1MB)", result);
        }

        [Fact]
        public void ExactlyOneMB_ReturnsMedium()
        {
            var file = CreateSizedFile(1024 * 1024); 
            var result = new SizeSortRule().GetSubFolderName(file);
            Assert.Equal("Medium (1MB-100MB)", result);
        }

        [Fact]
        public void HugeFile_ReturnsLarge()
        {
            var file = CreateSizedFile(1024L * 1024L * 500L); 
            var result = new SizeSortRule().GetSubFolderName(file);
            Assert.Equal("Large (100MB-1GB)", result);
        }

    }
}