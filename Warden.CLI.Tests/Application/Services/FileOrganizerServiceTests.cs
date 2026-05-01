using Moq;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Domain.Rules;

namespace Warden.CLI.Tests.Application.Services
{
    public class FileOrganizerServiceTests
    {
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly FileOrganizerService _service;

        public FileOrganizerServiceTests()
        {
            _mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            _service = new FileOrganizerService(_mockFileSystem.Object);
        }

        [Fact]
        public void OrganizeDirectory_WhenCalled_ReturnsReportWithAllFiles()
        {
            FileInfo[] files = { new FileInfo("fileA.txt"), new FileInfo("fileB.txt")};
            _mockFileSystem.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(files);
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            var report = _service.OrganizeDirectory("/fake/path", false, sortRules);

            Assert.Equal(2, report.Files.Count);
        }

        [Fact]
        public void OrganizeDirectory_WhenDryRunIsTrue_SetsDryRunOnReport()
        {
            FileInfo[] files = { new FileInfo("fileA.txt"), new FileInfo("fileB.txt")};
            _mockFileSystem.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(files);
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            string sourcePath = "/fake/path";
            string destinationPath = sourcePath + "/category";

            var report = _service.OrganizeDirectory(sourcePath, true, sortRules);

            Assert.True(report.IsDryRun);
            _mockFileSystem.Verify(m => m.MoveFile(sourcePath, destinationPath), Times.Never());
        }
        
        [Fact]
        public void OrganizeDirectory_WhenDirectoryIsEmpty_ReturnsEmptyReport()
        {
            FileInfo[] files = Array.Empty<FileInfo>();
            _mockFileSystem.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(files);
            
            var sortRules = new List<ISortRule> { new CategorySortRule() };

            var report = _service.OrganizeDirectory("/fake/path", false, sortRules);

            Assert.Empty(report.Files);
        }



    }
}






