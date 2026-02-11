using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Services;

namespace Warden.CLI.Tests.Application.Services
{
    public class AuditServiceTests
    {
        private readonly string _testDir;
        public AuditServiceTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "Warden_AuditService_" + Guid.NewGuid());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetRecentLogs_PastEntries_ReadsEntries()
        {
            var service = new AuditService(_testDir);

            List<LogEntry> originalLogs = new List<LogEntry>
            {

                new LogEntry{ FileName = "fake.txt", Action = "Moved" },
                new LogEntry{ FileName = "fake.txt (1)", Action = "Moved" }
            };

            foreach (var log in originalLogs)
            {
                service.AddEntry(log);
            }

            List<LogEntry> returnedLogs = service.GetRecentLogs(2);


            Assert.Equal(originalLogs[1].FileName, returnedLogs[0].FileName);
            Assert.Equal(originalLogs[1].Action, returnedLogs[0].Action);

            Assert.Equal(originalLogs[0].FileName, returnedLogs[1].FileName);
            Assert.Equal(originalLogs[0].Action, returnedLogs[1].Action);

        }

    }
}