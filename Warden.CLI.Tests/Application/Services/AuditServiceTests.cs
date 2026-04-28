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

        [Fact]
        public void GetRecentLogs_WhenLogFileDoesNotExist_ReturnsEmptyList()
        {
            var auditService = new AuditService(_testDir);

            var recentLogs = auditService.GetRecentLogs(1);

            Assert.Equal(new List<LogEntry>(), recentLogs);
        }

        [Fact]
        public void GetRecentLogs_WhenCalled_ReturnsCorrectAmount()
        {
            var auditService = new AuditService(_testDir);

            List<FileRecord> fileRecords = new List<FileRecord>
            {
                new FileRecord { FileName = "Notes.txt"},
                new FileRecord { FileName = "Notes (1).txt"},
                new FileRecord { FileName = "Notes (2).txt"},
                new FileRecord { FileName = "Notes (3).txt"},
                new FileRecord { FileName = "Notes (4).txt"}
            };

            string[] rulesApplied = { "category" };
            foreach (var record in fileRecords)
            {
                Guid guid = new Guid();
                auditService.AddFromRecord(record, guid, rulesApplied);
            }

            var recentLogs = auditService.GetRecentLogs(3);

            Assert.Equal(3, recentLogs.Count);
        }

        [Fact]
        public void GetRecentLogs_WhenCalled_ReturnsMostRecentFirst()
        {
            var auditService = new AuditService(_testDir);

            List<FileRecord> fileRecords = new List<FileRecord>
            {
                new FileRecord { FileName = "old.pdf"},
                new FileRecord { FileName = "recent.pdf"},
                new FileRecord { FileName = "new.pdf"}
            };

            string[] rulesApplied = { "category" };
            foreach (var record in fileRecords)
            {
                Guid guid = new Guid();
                auditService.AddFromRecord(record, guid, rulesApplied);
            }

            var recentLogs = auditService.GetRecentLogs(2);

            Assert.Equal(fileRecords[2].FileName, recentLogs[0].FileName);
            Assert.Equal(fileRecords[1].FileName, recentLogs[1].FileName);
            Assert.True(recentLogs[1].TimeStamp < recentLogs[0].TimeStamp);
        }

        [Fact]
        public void GetRecentLogs_WhenLimitExceedsTotalEntries_ReturnAllEntries()
        {
            var auditService = new AuditService(_testDir);

            List<FileRecord> fileRecords = new List<FileRecord>
            {
                new FileRecord { FileName = "first.csv"},
                new FileRecord { FileName = "second.json"},
            };

            string[] rulesApplied = { "category" };
            foreach (var record in fileRecords)
            {
                Guid guid = new Guid();
                auditService.AddFromRecord(record, guid, rulesApplied);
            }

            var recentLogs = auditService.GetRecentLogs(100);

            Assert.Equal(2, recentLogs.Count);
        }

    }
}