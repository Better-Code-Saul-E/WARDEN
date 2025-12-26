using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Factories;
using Warden.CLI.Domain.Rules;

namespace Warden.CLI.Application.Services
{
    public class FileOrganizerService : IFileOrganizerService
    {
        private readonly IFileSystem _fileSystem;

        public FileOrganizerService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public OrganizeReport Organize(string directoryPath, bool isAuditMode, string[] orderBy)
        {
            if (!_fileSystem.DirectoryExists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            var rules = new List<ISortRule>();
            foreach (var ruleName in orderBy)
            {
                var rule = SortRuleFactory.Create(ruleName);

                if (rule != null)
                {
                    rules.Add(rule);
                }
            }


            var result = new OrganizeReport
            {
                IsAuditMode = isAuditMode,
                Files = new List<FileRecord>()
            };

            var files = _fileSystem.GetFiles(directoryPath);
            foreach (var file in files)
            {
                var fileResult = ProcessFile(file, directoryPath, isAuditMode, rules);
                result.Files.Add(fileResult);
            }

            return result;
        }
        public FileRecord ProcessFile(FileInfo file, string targetDirectory, bool isAuditMode, List<ISortRule> rules)
        {
            var currentPath = targetDirectory;

            foreach (var rule in rules)
            {
                var subFolder = rule.GetSubFolderName(file);
                currentPath = Path.Combine(currentPath, subFolder);
            }
            var destinationFolder = currentPath;

            var dto = new FileRecord
            {
                FileName = file.Name,
                Category = "Sorted",
                Success = true,
            };

            if (isAuditMode)
            {
                dto.Action = "Will Move";
                return dto;
            }

            try
            {
                if (!_fileSystem.DirectoryExists(destinationFolder))
                {
                    _fileSystem.CreateDirectory(destinationFolder);
                }

                var uniqueDestinationPath = GetUniqueFilePath(destinationFolder, file.Name);
                var finalFileName = Path.GetFileName(uniqueDestinationPath);
                var wasRenamed = !finalFileName.Equals(file.Name, StringComparison.OrdinalIgnoreCase);

                _fileSystem.MoveFile(file.FullName, uniqueDestinationPath);


                if (wasRenamed)
                {
                    dto.Action = $"Renamed to {finalFileName}";
                    dto.Success = true;
                }
                else
                {
                    dto.Action = "Moved";
                    dto.Success = true;
                }
            }
            catch (Exception ex)
            {
                dto.Action = $"Error: {ex.Message}";
                dto.Success = false;
            }

            return dto;
        }
        private string GetUniqueFilePath(string folderPath, string fileName)
        {
            var fullPath = Path.Combine(folderPath, fileName);

            if (!_fileSystem.FileExists(fullPath))
            {
                return fullPath;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var counter = 1;

            while (true)
            {
                var newFileName = $"{fileNameWithoutExtension} ({counter}){extension}";
                var newFullPath = Path.Combine(folderPath, newFileName);

                if (!_fileSystem.FileExists(newFullPath))
                {
                    return newFullPath;
                }

                counter++;
            }
        }
    }
}