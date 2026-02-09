using Moq;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Commands;
using Xunit;

namespace Warden.CLI.Tests
{
    public class AuditCommandTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Execute_LogsAvailible_CallsFormatterRenderTable()
        {
            var mockService = new Mock<IAuditService>();
            var mockConsole = new Mock<IAuditFormatter>();

            var fakeEntries = new List<LogEntry>
            {
                new LogEntry
                {
                    FileName = "fake.txt",
                    Action = "Moved"
                }
            };

            mockService.Setup(s => s.GetRecentLogs(
                It.IsAny<int>()
            )).Returns(fakeEntries);

            var command = new AuditCommand(mockService.Object, mockConsole.Object);
            var settings = new AuditSettings
            {
                Limit = 2
            };

            command.Execute(null!, settings, CancellationToken.None);
            
            mockConsole.Verify(f => f.RenderTable(
                fakeEntries
            ), Times.Once);
        }
    }
}