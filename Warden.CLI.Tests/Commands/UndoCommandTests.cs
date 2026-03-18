using System.ComponentModel;
using Moq;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Commands;
using Warden.CLI.Output;

namespace Warden.CLI.Tests.Commands
{
    public class UndoCommandTests 
    {

        [Fact]
        [Trait("Category", "Unit")]
        public void Execute_LogEntry_RestoresFile()
        {
            var mockService = new Mock<IAuditService>();
            var mockFileSystem = new Mock<IFileSystem>();
            var mockConsole = new Mock<IConsoleFormatter>();

            List<LogEntry> entryList = new List<LogEntry>
            {
                new LogEntry
                {
                    FileName = "test.txt",
                    SourcePath = "original.txt",
                    DestinationPath = "moved.txt"
                }
            };

            mockService.Setup(s => s.GetLastBatch()).Returns(entryList);
            mockFileSystem.Setup(s => s.FileExists(
                "moved.txt"
            )).Returns(true);
            mockConsole.Setup(c => c.RenderInfo(It.IsAny<string>()));

            var command = new UndoCommand(mockService.Object, mockFileSystem.Object, mockConsole.Object);

            var settings = new UndoSettings
            {
                Force = true
            };

            command.Execute(null!, settings, CancellationToken.None);

            mockFileSystem.Verify(s => s.MoveFile(
                "moved.txt",
                "original.txt"
            ), Times.Once);

            mockConsole.Verify(c => c.RenderInfo(
                "Restored 'test.txt'"
            ), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Execute_MissingFile_SkipsRestore()
        {
            var mockService = new Mock<IAuditService>();
            var mockFileSystem = new Mock<IFileSystem>();
            var mockConsole = new Mock<IConsoleFormatter>();

            List<LogEntry> entryList = new List<LogEntry>
            {
                new LogEntry
                {
                    FileName = "test.txt",
                    SourcePath = "original.txt",
                    DestinationPath = "moved.txt"
                }
            };

            mockService.Setup(s => s.GetLastBatch()).Returns(entryList);
            mockFileSystem.Setup(s => s.FileExists(
                "moved.txt"
            )).Returns(false);

            var command = new UndoCommand(mockService.Object, mockFileSystem.Object, mockConsole.Object);

            var settings = new UndoSettings
            {
                Force = true
            };

            command.Execute(null!, settings, CancellationToken.None);

            mockFileSystem.Verify(s => s.MoveFile(
                "moved.txt",
                "original.txt"
            ), Times.Never);

            mockConsole.Verify(c => c.RenderWarning(
                It.Is<string>(s => s.Contains("skipping")),
                It.IsAny<string>()
            ), Times.Once);
        }
    }
}