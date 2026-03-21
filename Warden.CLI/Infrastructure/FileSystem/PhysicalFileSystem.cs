using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Infrastructure.FileSystem
{
    public class PhysicalFileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
        public FileInfo[] GetFiles(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }
            
            var directoryInfo = new DirectoryInfo(directoryPath);

            return directoryInfo.GetFiles();
        }
        public void MoveFile(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }
    }
}