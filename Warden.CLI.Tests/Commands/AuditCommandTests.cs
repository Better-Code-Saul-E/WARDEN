using System.Text.Json;
using Moq;
using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Commands;
using Xunit;

namespace Warden.CLI.Tests
{
    public class AuditCommandTests
    {
        private readonly Mock<IAuditService> _mockService;
        private readonly Mock<IAuditFormatter> _mockFormatter;
        private readonly AuditCommand _command;
        private readonly AuditSettings _settings;


        public AuditCommandTests()
        {
            _mockService = new Mock<IAuditService>();
            _mockFormatter = new Mock<IAuditFormatter>();
            _command = new AuditCommand(_mockService.Object, _mockFormatter.Object);
            _settings = new AuditSettings { Limit = 10 };
        }


        [Fact]
        public void Execute_LogsAvailible_CallsRenderTable()
        {
            var fakeEntries = new List<LogEntry>
            {
                new LogEntry
                {
                    FileName = "fake.txt",
                    Action = "Moved"
                }
            };

            _mockService.Setup(s => s.GetRecentLogs(It.IsAny<int>()))
                .Returns(fakeEntries);

            var result = _command.Execute(null!, _settings, CancellationToken.None);

            _mockFormatter.Verify(f => f.RenderInfo(It.IsAny<string>()), Times.Never);
            _mockFormatter.Verify(f => f.RenderTable(It.IsAny<List<LogEntry>>()), Times.Once);
            Assert.Equal(0, result);
        }

        [Fact]
        public void Execute_NoLogsAvailible_CallsRenderInfo()
        {
            var fakeEntries = new List<LogEntry>();

            _mockService.Setup(s => s.GetRecentLogs(It.IsAny<int>()))
                .Returns(fakeEntries);

            var result = _command.Execute(null!, _settings, CancellationToken.None);

            _mockFormatter.Verify(f => f.RenderInfo(It.Is<string>(s => s.Contains("No audit logs found."))), Times.Once);
            _mockFormatter.Verify(f => f.RenderTable(It.IsAny<List<LogEntry>>()), Times.Never);
            Assert.Equal(0, result);
        }

        [Fact]
        public void Execute_WhenAuditServiceThrows_RenderError()
        {
            _mockService.Setup(s => s.GetRecentLogs(It.IsAny<int>()))
                .Throws(new JsonException());

            var result = _command.Execute(null!, _settings, CancellationToken.None);

            _mockFormatter.Verify(f => f.RenderError(It.Is<string>(s => s.Contains("fetching audit logs")), It.IsAny<string>()), Times.Once);
            _mockFormatter.Verify(f => f.RenderTable(It.IsAny<List<LogEntry>>()), Times.Never);
            Assert.Equal(10, result);
        }

        [Fact]
        public void Execute_WhenCalled_PassesLimitSettingToGetRecentLogs()
        {
            const int expectedLimit = 50;

            _mockService.Setup(s => s.GetRecentLogs(expectedLimit))
                       .Returns(new List<LogEntry>());

            var settings = new AuditSettings { Limit = expectedLimit };

            _command.Execute(null!, settings , CancellationToken.None);

            _mockService.Verify(s => s.GetRecentLogs(expectedLimit), Times.Once());
        }
    }
}