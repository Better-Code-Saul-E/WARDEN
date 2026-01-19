using System.IO;

namespace Warden.CLI.Application.Interfaces
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        bool FileExists(string filePath);
        FileInfo[] GetFiles(string directoryPath);
        void MoveFile(string sourcePath, string destinationPath);
    }
}