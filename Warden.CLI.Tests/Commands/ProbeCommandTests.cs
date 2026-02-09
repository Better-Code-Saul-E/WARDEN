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
    public class ProbeCommandTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _sandboxDir;

        public ProbeCommandTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "Warden_Probe_" + Guid.NewGuid());
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
        public void Execute_DefaultSettings_CallsOrganizeWithDryRunTrue()
        {

            var mockService = new Mock<IFileOrganizerService>();
            var mockAudit = new Mock<IAuditService>();
            var mockConsole = new Mock<IConsole>();

            mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));

            var fakeReport = new OrganizeReport
            {
                IsDryRun = true,
                Files = new List<FileRecord>()
            };

            mockService.Setup(s => s.Organize(
                It.IsAny<string>(),
                true,
                It.IsAny<string[]>())
            ).Returns(fakeReport);

            var formatter = new ConsoleFormatter(mockConsole.Object);
            var handler = new OrganizeCommandHandler(mockService.Object, mockAudit.Object, formatter);
            var command = new ProbeCommand(handler);

            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "category" }
            };

            command.Execute(null!, settings, CancellationToken.None);

            mockService.Verify(s => s.Organize(
                It.IsAny<string>(),
                true,
                It.IsAny<string[]>())
            , Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Execute_FilesExistOnDisk_DoesNotMoveFiles()
        {
            var fileSystem = new PhysicalFileSystem();
            var service = new FileOrganizerService(fileSystem);
            var mockAudit = new Mock<IAuditService>();
            var mockConsole = new Mock<IConsole>();
            var formatter = new ConsoleFormatter(mockConsole.Object);

            var handler = new OrganizeCommandHandler(service, mockAudit.Object, formatter);

            var command = new ProbeCommand(handler);
            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "category" }
            };

            CreatTestFile("testing.txt");

            command.Execute(null!, settings, CancellationToken.None);

            var expectedPath = Path.Combine(_sandboxDir, "Documents", "testing.txt");

            Assert.False(File.Exists(expectedPath), $"File should not be in {expectedPath}");
            Assert.True(File.Exists(Path.Combine(_sandboxDir, "testing.txt")), "File should not be moved from the source");
        }

        private void CreatTestFile(string name)
        {
            var path = Path.Combine(_sandboxDir, name);
            File.WriteAllText(path, "content");
        }
    }
}