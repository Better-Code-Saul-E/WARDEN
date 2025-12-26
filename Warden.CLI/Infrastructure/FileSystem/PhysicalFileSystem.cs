using Warden.CLI.Application.Interfaces;

namespace Warden.CLI.Infrastructure.FileSystem
{
    public class PhysicalFileSystem : IFileSystem
    {
        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }
        public bool FileExists(string directoryPath)
        {
            return File.Exists(directoryPath);
        }
        public FileInfo[] GetFiles(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            // if the directory does not exists it might throw an exception
            // Either make the caller handle the error or implement a try.catch
            return directoryInfo.GetFiles();
        }
        public void MoveFile(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }
    }
}