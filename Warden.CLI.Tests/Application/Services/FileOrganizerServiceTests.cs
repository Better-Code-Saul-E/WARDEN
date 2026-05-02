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
            FileInfo[] files = { new FileInfo("fileA.txt"), new FileInfo("fileB.txt") };
            _mockFileSystem.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(files);
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);

            var sortRules = new List<ISortRule> { new CategorySortRule() };
            var report = _service.OrganizeDirectory("/fake/path", false, sortRules);

            Assert.Equal(2, report.Files.Count);
        }

        [Fact]
        public void OrganizeDirectory_WhenDryRunIsTrue_SetsDryRunOnReport()
        {
            FileInfo[] files = { new FileInfo("fileA.txt"), new FileInfo("fileB.txt") };
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

        [Fact]
        public void OrganizeFile_WhenFileNameStartsWithDot_ReturnsSkippedRecord()
        {
            FileInfo file = new FileInfo(".obj");
            var sortRules = new List<ISortRule> { new CategorySortRule() };

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.False(result.Success);
            Assert.Equal("Skipped (Hidden File)", result.Action);
            Assert.Equal(result.SourcePath, result.DestinationPath);
        }

        [Fact]
        public void OrganizeFile_WhenDryRun_DoesNotCallMoveFile()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);

            var result = _service.OrganizeFile(file, "/fake/path", true, sortRules);

            Assert.Equal("Will Move", result.Action);
            Assert.Equal("/fake/path/Code/snake.py", result.DestinationPath);
            _mockFileSystem.Verify(m => m.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void OrganizeFile_WhenNotDryRun_CallsMoveFile()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.MoveFile(It.IsAny<string>(), It.IsAny<string>()));

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.True(result.Success);
            Assert.Equal("snake.py", result.FileName);
            Assert.Equal("Moved", result.Action);
            Assert.Equal("/fake/path/snake.py", result.SourcePath);
            Assert.Equal("/fake/path/Code/snake.py", result.DestinationPath);
            _mockFileSystem.Verify(m => m.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void OrganizeFile_WhenDestinationDirectoryDoesNotExist_CreatesDirectory()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.CreateDirectory(It.IsAny<string>()));
            _mockFileSystem.Setup(f => f.MoveFile(It.IsAny<string>(), It.IsAny<string>()));

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.Equal("/fake/path/Code/snake.py", result.DestinationPath);
            _mockFileSystem.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void OrganizeFile_WhenMoveFileThrows_ReturnsFailedRecord()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");
            var sortRules = new List<ISortRule> { new YearSortRule(), new MonthSortRule() };
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.MoveFile(It.IsAny<string>(), It.IsAny<string>())).Throws(new IOException("Access to the path is denied."));

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.False(result.Success);
            Assert.StartsWith("Error:", result.Action);
        }

        [Fact]
        public void OrganizeFile_WhenMultipleRulesProvided_BuildsNestedDestinationPath()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");

            var mockYearRule = new Mock<ISortRule>();
            mockYearRule.Setup(r => r.GetSubFolderName(It.IsAny<FileInfo>())).Returns("2024");

            var mockMonthRule = new Mock<ISortRule>();
            mockMonthRule.Setup(r => r.GetSubFolderName(It.IsAny<FileInfo>())).Returns("January");

            var sortRules = new List<ISortRule> { mockYearRule.Object, mockMonthRule.Object };
            _mockFileSystem.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.MoveFile(It.IsAny<string>(), It.IsAny<string>()));

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.True(result.Success);
            Assert.Equal("Moved", result.Action);
            Assert.Equal("/fake/path/snake.py", result.SourcePath);
            Assert.Equal("/fake/path/2024/January/snake.py", result.DestinationPath);
        }

        [Fact]
        public void OrganizeFile_WhenConflictExists_AppendsCounterToFileName()
        {
            FileInfo file = new FileInfo("/fake/path/snake.py");
            var sortRules = new List<ISortRule> { new CategorySortRule() };
            _mockFileSystem.SetupSequence(f => f.FileExists(It.IsAny<string>())).Returns(true).Returns(false);
            _mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.MoveFile(It.IsAny<string>(), It.IsAny<string>()));

            var result = _service.OrganizeFile(file, "/fake/path", false, sortRules);

            Assert.True(result.Success);
            Assert.Equal("snake.py", result.FileName);
            Assert.Equal("Moved and Renamed to snake (1).py", result.Action);
            Assert.Equal("/fake/path/snake.py", result.SourcePath);
            Assert.Equal("/fake/path/Code/snake (1).py", result.DestinationPath);
            _mockFileSystem.Verify(m => m.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

    }
}