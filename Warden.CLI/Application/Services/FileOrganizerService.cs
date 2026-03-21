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

        public OrganizeReport OrganizeDirectory(string sourceDirectory, bool isDryRun, List<ISortRule> rules)
        {
            var result = new OrganizeReport
            {
                IsDryRun = isDryRun,
                Files = new List<FileRecord>()
            };

            var files = _fileSystem.GetFiles(sourceDirectory);

            foreach (var file in files)
            {
                var fileResult = OrganizeFile(file, sourceDirectory, isDryRun, rules);

                result.Files.Add(fileResult);
            }

            return result;
        }
        public FileRecord OrganizeFile(FileInfo file, string sourceDirectory, bool isDryRun, List<ISortRule> rules)
        {
            if (file.Name.StartsWith("."))
            {
                return new FileRecord
                {
                    FileName = file.Name,
                    SourcePath = file.FullName,
                    DestinationPath = file.FullName, 
                    Success = false,
                    Action = "Skipped (Hidden File)"
                };
            }

            return ProcessFile(file, sourceDirectory, isDryRun, rules);
        }
        private FileRecord ProcessFile(FileInfo file, string sourceDirectory, bool isDryRun, List<ISortRule> rules)
        {
            var currentPath = sourceDirectory;
            var displayPath = "";
            foreach (var rule in rules)
            {
                var subFolder = rule.GetSubFolderName(file);
                displayPath = Path.Combine(displayPath, subFolder);
                currentPath = Path.Combine(currentPath, subFolder);
            }

            var destinationFolder = currentPath;
            var uniqueDestinationPath = GetUniqueFilePath(destinationFolder, file.Name);

            var dto = new FileRecord
            {
                FileName = file.Name,
                SourcePath = file.FullName,
                Success = true,
                DestinationPath = Path.GetFullPath(uniqueDestinationPath)
            };

            var projectedFileName = Path.GetFileName(uniqueDestinationPath);

            if (isDryRun)
            {
                if (!projectedFileName.Equals(file.Name, StringComparison.OrdinalIgnoreCase))
                {
                    dto.Action = $"Will Move and Rename to {projectedFileName}";
                }
                else
                {
                    dto.Action = "Will Move";

                }

                return dto;
            }

            try
            {
                if (!_fileSystem.DirectoryExists(destinationFolder))
                {
                    _fileSystem.CreateDirectory(destinationFolder);
                }

                var wasRenamed = !projectedFileName.Equals(file.Name, StringComparison.OrdinalIgnoreCase);

                _fileSystem.MoveFile(file.FullName, uniqueDestinationPath);

                if (wasRenamed)
                {
                    dto.Action = $"Moved and Renamed to {projectedFileName}";
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