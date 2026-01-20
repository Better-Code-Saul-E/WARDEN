using Spectre.Console.Rendering;
using Warden.CLI.Application.Factories;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Commands;
using Warden.CLI.Handlers;
using Warden.CLI.Infrastructure.FileSystem;
using Warden.CLI.Output;
using Xunit;

namespace Warden.CLI.Tests.Application.Services
{
    public class DryRunTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _sandboxDir;
        public DryRunTests()
        {

            _testDir = Path.Combine(Path.GetTempPath(), "Warden_Watch_Test_" + Guid.NewGuid());
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
        public void ProcessFile_Status_ReturnsWillMove()
        {
            var fileSystem = new PhysicalFileSystem();
            var service = new FileOrganizerService(fileSystem);

            var filePath = Path.Combine(_sandboxDir, "Testing.txt");
            File.WriteAllText(filePath, "Data");
            FileInfo fileInfo = new FileInfo(filePath);


            var rules = new List<ISortRule>
            {
                SortRuleFactory.Create("category")!
            };


            var fileRecord = service.ProcessFile(fileInfo, _sandboxDir, true, rules);

            Assert.Equal("Will Move", fileRecord.Action);
        }

        [Fact]
        public void DryRunOranigize_File_DoesNotMoveFile()
        {
            var fileSystem = new PhysicalFileSystem();
            var console = new FakeConsole();
            var service = new FileOrganizerService(fileSystem);
            var formatter = new ConsoleFormatter(console);
            var organizeHandler = new OrganizeCommandHandler(service, formatter);
            var command = new ProbeCommand(organizeHandler);

            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "category" }
            };

            var filePath = Path.Combine(_sandboxDir, "testing.txt");
            File.WriteAllText(filePath, "This is a test file");

            var cts = new CancellationTokenSource();
            command.Execute(null!, settings, cts.Token);
            cts.Cancel();

            Assert.True(File.Exists(filePath), "Path did not remain the same.");

            var destinationPath = Path.Combine(_sandboxDir, "Documents", "testing.txt");
            Assert.False(File.Exists(destinationPath), "File is copied to destination path.");
        }
        private class FakeConsole : IConsole
        {
            public void Write(IRenderable value)
            {
            }

            public void WriteLine(string text)
            {
            }
        }
    }
}