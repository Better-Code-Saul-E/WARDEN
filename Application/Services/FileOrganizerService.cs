using Warden.CLI.Application.DTOs;
using Warden.CLI.Application.Interfaces;
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

        public OrganizeReport Organize(string directoryPath, bool isAuditMode)
        {
            if (!_fileSystem.DirectoryExists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            var result = new OrganizeReport
            {
                IsAuditMode = isAuditMode,
                Files = new List<FileRecord>()
            };
            var files = _fileSystem.GetFiles(directoryPath);

            foreach (var file in files)
            {
                var fileResult = ProcessFile(file, directoryPath, isAuditMode);
                result.Files.Add(fileResult);
            }

            return result;
        }
        public FileRecord ProcessFile(FileInfo file, string targetDirectory, bool isAuditMode)
        {
            var category = FileCategoryRules.GetCategory(file.Extension);

            var cleanedExtension = file.Extension.TrimStart('.').ToLowerInvariant();
            if (string.IsNullOrEmpty(cleanedExtension))
            {
                cleanedExtension = "no_ext";
            }

            var destinationFolder = Path.Combine(targetDirectory, category, cleanedExtension);
            var destinationFilePath = Path.Combine(destinationFolder, file.Name);

            var dto = new FileRecord
            {
                FileName = file.Name,
                Category = category,
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
                if (_fileSystem.FileExists(destinationFilePath))
                {
                    dto.Action = "Skipped (Exists)";
                    dto.Success = false;
                }
                else
                {
                    _fileSystem.MoveFile(file.FullName, destinationFilePath);
                    dto.Action = "Moved";
                }
            }
            catch (Exception ex)
            {
                dto.Action = $"Error: {ex.Message}";
                dto.Success = false;
            }
            
            return dto;
        }
    }
}