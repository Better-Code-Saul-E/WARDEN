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
        public void AddFromRecord_WhenCalled_WritesEntryToLog()
        {
            var auditService = new AuditService(_testDir);

            List<FileRecord> recordsToBeLogged = new List<FileRecord>
            {
                new FileRecord
                {
                    FileName = "Family.png",
                    Action = "Moved",
                    SourcePath = "Notes/Algebra.doc.",
                    DestinationPath = "Notes/Images/Algebra.doc",
                    Success = true
                },
                new FileRecord
                {
                    FileName = "JBL_Speaker_Manual.pdf",
                    Action = "Moved",
                    SourcePath = "Manuals/JBL_Speaker_Manual.pdf",
                    DestinationPath = "Manuals/Documents/JBL_Speaker_Manual.pdf",
                    Success = true
                },
            };

            Guid guid = new Guid();
            string[] rulesApplied = { "category" };

            foreach (var fileRecord in recordsToBeLogged)
            {
                auditService.AddFromRecord(fileRecord, guid, rulesApplied);
            }

            var recentLog = auditService.GetRecentLogs(1);

            Assert.Equal(recordsToBeLogged[1].FileName, recentLog[0].FileName);
            Assert.Equal(recordsToBeLogged[1].Action, recentLog[0].Action);
            Assert.Equal(recordsToBeLogged[1].SourcePath, recentLog[0].SourcePath);
            Assert.Equal(recordsToBeLogged[1].DestinationPath, recentLog[0].DestinationPath);
        }

        [Fact]
        public void AddFromRecord_WhenCalled_CorrectlyJoinsRulesApplied()
        {
            var auditService = new AuditService(_testDir);

            FileRecord recordToBeLogged = new FileRecord
            {
                FileName = "JBL_Speaker_Manual.pdf",
                Action = "Moved",
                SourcePath = "Manuals/JBL_Speaker_Manual.pdf",
                DestinationPath = "Manuals/Documents/pdf/2026/JBL_Speaker_Manual.pdf",
                Success = true
            };

            Guid guid = new Guid();
            string[] rulesApplied = { "category", "extension", "year" };

            auditService.AddFromRecord(recordToBeLogged, guid, rulesApplied);

            var recentLog = auditService.GetRecentLogs(1);

            Assert.Equal(string.Join(", ", rulesApplied), recentLog[0].RuleApplied);
        }

         [Fact]
        public void AddFromRecord_WhenCalled_StoresCorrectBatchId()
        {
            var auditService = new AuditService(_testDir);

            FileRecord recordToBeLogged = new FileRecord
            {
                FileName = "JBL_Speaker_Manual.pdf",
                Action = "Moved",
                SourcePath = "Manuals/JBL_Speaker_Manual.pdf",
                DestinationPath = "Manuals/Documents/JBL_Speaker_Manual.pdf",
                Success = true
            };

            Guid guid = new Guid();
            string[] rulesApplied = { "category" };

            auditService.AddFromRecord(recordToBeLogged, guid, rulesApplied);

            var recentLog = auditService.GetRecentLogs(1);

            Assert.Equal(guid, recentLog[0].BatchId);
        }

    }
}