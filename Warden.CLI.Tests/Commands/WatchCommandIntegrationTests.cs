using System.Threading.Tasks;
using Spectre.Console.Cli;
using Warden.CLI.Application.Services;
using Warden.CLI.Commands;
using Warden.CLI.Infrastructure.FileSystem;
using Xunit;

namespace Warden.CLI.Tests.Commands
{
    public class WatchCommandIntegrationTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _sandboxDir;

        public WatchCommandIntegrationTests()
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
        public async Task ExecuteAsync_ShouldSortExistingAndNewFiles()
        {
            var fileSystem = new PhysicalFileSystem();
            var service = new FileOrganizerService(fileSystem);
            var command = new WatchCommand(service);

            var settings = new SortSettings
            {
                SourcePath = _sandboxDir,
                OrderBy = new[] { "extension" }
            };

            var cts = new CancellationTokenSource();

            var watchTask = Task.Run(async () =>
            {
                await command.ExecuteAsync(null!, settings, cts.Token);
            });

            var existingFile = Path.Combine(_sandboxDir, "old_photo.jpg");
            File.WriteAllText(existingFile, "Old Data");

            await Task.Delay(1000);

            var newFile = Path.Combine(_sandboxDir, "new_song.mp3");
            File.WriteAllText(newFile, "New Data");

            await Task.Delay(1500);

            cts.Cancel();

            try
            {
                await watchTask;
            }
            catch (TaskCanceledException) { }

            var expectedOldPath = Path.Combine(_sandboxDir, "jpg", "old_photo.jpg");
            Assert.True(File.Exists(expectedOldPath), "Initial Sweep failed to move existing file.");

            var expectedNewPath = Path.Combine(_sandboxDir, "mp3", "new_song.mp3");
            Assert.True(File.Exists(expectedNewPath), "Watcher failed to catch new file.");
        }
    }
}