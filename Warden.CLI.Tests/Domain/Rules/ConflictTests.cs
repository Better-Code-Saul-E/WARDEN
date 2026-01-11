using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Infrastructure.FileSystem;

namespace Warden.CLI.Tests.Domain.Rules
{
    public class ConflictTests : IDisposable
    {
        private readonly string _testDir;
        public ConflictTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "Warden_Conflict_Test_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDir);
        }
        public void Dispose()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Fact]
        public void MoveFile_ShouldNotCrash_WhenFileExists()
        {
            var sourceFile = Path.Combine(_testDir, "exists.txt");
            var destFolder = Path.Combine(_testDir, "Sorted");
            var destFile = Path.Combine(destFolder, "exists.txt");

            Directory.CreateDirectory(destFolder);
            File.WriteAllText(sourceFile, "The new file");
            File.WriteAllText(destFile, "The existing file");

            var fileSystem = new PhysicalFileSystem();
            var service = new FileOrganizerService(fileSystem);

            var fileInfo = new FileInfo(sourceFile);
            var result = service.ProcessFile(fileInfo, destFolder, false, new List<ISortRule>());

            Assert.True(result.Success);
            Assert.Contains("Renamed to", result.Action); 
            
            var expectedRenamedPath = Path.Combine(destFolder, "exists (1).txt");
            Assert.True(File.Exists(expectedRenamedPath), "File should have been renamed to 'exists (1).txt'");
        }

    }
}