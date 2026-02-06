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
            _testDir = Path.Combine(Path.GetTempPath(), "Warden_Conflict_" + Guid.NewGuid());
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
        public void ProcessFile_DestinationExists_RenamesWithIncrement()
        {
            var sourceFile = Path.Combine(_testDir, "report.txt");
            var destFolder = Path.Combine(_testDir, "Docs");
            var destFile = Path.Combine(destFolder, "report.txt");

            Directory.CreateDirectory(destFolder);
            File.WriteAllText(sourceFile, "New Content");
            File.WriteAllText(destFile, "Old Content"); 

            var service = new FileOrganizerService(new PhysicalFileSystem());
            var fileInfo = new FileInfo(sourceFile);

            var result = service.ProcessFile(fileInfo, destFolder, false, new List<ISortRule>());
            Assert.True(result.Success);

            var expectedPath = Path.Combine(destFolder, "report (1).txt");
            Assert.True(File.Exists(expectedPath), "Should have created 'report (1).txt'");

            Assert.True(File.Exists(destFile), "Original file should still be there");
        }
        
        [Fact]
        public void ProcessFile_MultiConflict_RenamesToNextAvailable()
        {
            var sourceFile = Path.Combine(_testDir, "image.png");
            var destFolder = Path.Combine(_testDir, "Images");
            
            Directory.CreateDirectory(destFolder);
            File.WriteAllText(sourceFile, "New Image");
            File.WriteAllText(Path.Combine(destFolder, "image.png"), "Taken");
            File.WriteAllText(Path.Combine(destFolder, "image (1).png"), "Taken also");

            var service = new FileOrganizerService(new PhysicalFileSystem());
            var fileInfo = new FileInfo(sourceFile);

            var result = service.ProcessFile(fileInfo, destFolder, false, new List<ISortRule>());

            Assert.True(result.Success);

            var expectedPath = Path.Combine(destFolder, "image (2).png");
            Assert.True(File.Exists(expectedPath), "Should have skipped (1) and created 'image (2).png'");
        }

    }
}