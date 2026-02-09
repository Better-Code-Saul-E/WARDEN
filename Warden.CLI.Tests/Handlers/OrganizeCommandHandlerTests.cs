using Moq;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Commands;
using Warden.CLI.Handlers;
using Warden.CLI.Infrastructure.FileSystem;
using Warden.CLI.Output;
using Xunit;

namespace Warden.CLI.Tests
{
    public class OrganizeCommandHandlerTests 
    {

        [Fact]
        [Trait("Category", "Unit")]
        public void ProcessRequest_SingleFile_LogsEntry()
        {
            var mockService = new Mock<IFileOrganizerService>();
            var mockAudit = new Mock<IAuditService>();
            var mockConsole = new Mock<IConsole>();
            var formatter = new ConsoleFormatter(mockConsole.Object);

            var fileList = new List<FileRecord>
            {
                new FileRecord
                {
                    FileName = "test.txt",
                    Action = "Moved"
                }
            };

            var fakeReport = new OrganizeReport
            {
                IsDryRun = false,
                Files = fileList
            };

            mockService.Setup(s => s.Organize(
                It.IsAny<string>(),
                false,
                It.IsAny<string[]>()))
                .Returns(fakeReport);

            var handler = new OrganizeCommandHandler(mockService.Object, mockAudit.Object, formatter);

            handler.ProcessRequest("any/path", false, new[] { "category" });

            mockAudit.Verify(s => 
                s.AddEntry(It.Is<LogEntry>(log => log.FileName == "test.txt"))
                , Times.Once);
        }
        
    }
}