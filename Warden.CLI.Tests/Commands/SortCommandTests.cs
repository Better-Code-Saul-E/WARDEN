using Moq;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Commands;
using Warden.CLI.Handlers;
using Warden.CLI.Infrastructure.FileSystem;
using Warden.CLI.Output;
using Xunit;

namespace Warden.CLI.Tests.Commands
{
    public class SortCommandTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _sandboxDir;

        public SortCommandTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "Warden_Sort_" + Guid.NewGuid());
            _sandboxDir = Path.Combine(_testDir, "Sandbox");
            Directory.CreateDirectory(_sandboxDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Execute_DefaultSettings_CallsOrganizeWithDryRunFalse()
        {

            var mockService = new Mock<IFileOrganizerService>();
            var mockAudit = new Mock<IAuditService>();
            var mockConsole = new Mock<IConsole>();

            mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));

            var fakeReport = new OrganizeReport
            {
                IsDryRun = false,
                Files = new List<FileRecord>()
            };

            mockService.Setup(s => s.Organize(
                It.IsAny<string>(),
                false,
                It.IsAny<string[]>())
            ).Returns(fakeReport);

            var formatter = new ConsoleFormatter(mockConsole.Object);
            var handler = new OrganizeCommandHandler(mockService.Object, mockAudit.Object, formatter);
            var command = new SortCommand(handler);

            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "category" }
            };

            command.Execute(null!, settings, CancellationToken.None);

            mockService.Verify(s => s.Organize(
                It.IsAny<string>(),
                false,
                It.IsAny<string[]>())
            , Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Execute_FilesExistsOnDisk_MovesFilesToTarget()
        {
            var fileSystem = new PhysicalFileSystem();
            var service = new FileOrganizerService(fileSystem);
            var mockAudit = new Mock<IAuditService>();
            var mockConsole = new Mock<IConsole>();
            var formatter = new ConsoleFormatter(mockConsole.Object);

            var handler = new OrganizeCommandHandler(service, mockAudit.Object, formatter);

            var command = new SortCommand(handler);
            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "category" }
            };

            CreatTestFile("testing.txt");

            command.Execute(null!, settings, CancellationToken.None);

            var expectedPath = Path.Combine(_sandboxDir, "Documents", "testing.txt");

            Assert.True(File.Exists(expectedPath), $"File should be in {expectedPath}");
            Assert.False(File.Exists(Path.Combine(_sandboxDir, "testing.txt")), "File should be moved from the source");
        }

        private void CreatTestFile(string name)
        {
            var path = Path.Combine(_sandboxDir, name);
            File.WriteAllText(path, "content");
        }
    }
}