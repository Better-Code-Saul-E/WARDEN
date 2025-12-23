using System.IO;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileSystem
    {
        bool DirectoryExists(string directoryPath);
        void CreateDirectory(string directoryPath);
        bool FileExists(string directoryPath);
        FileInfo[] GetFiles(string directoryPath);
        void MoveFile(string sourcePath, string destinationPath);
    }
}