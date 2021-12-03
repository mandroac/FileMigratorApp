using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace FileMigrator
{
    public class FileMigrationService
    {
        private readonly IFileSystem _fileSystem;
        public FileMigrationService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public string GenerateUniqueFileName(string path)
        {
            int counter = 0;
            var fileInfo = _fileSystem.FileInfo.FromFileName(path);
            string pathWithNoFileExtension = fileInfo.FullName.Remove(fileInfo.FullName.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
            string tempPath;
            
            do
            {
                tempPath = pathWithNoFileExtension + $" ({++counter})";
            } while (_fileSystem.File.Exists(tempPath));
            return tempPath + fileInfo.Extension;
        }

        public IEnumerable<string> GetFileNamesFromDirectory(string startDirectoryPath, string expectedFileDirectory)
        {
            IEnumerable<string> returnList = new List<string>();
            var directoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(startDirectoryPath);

            if (directoryInfo.Name == expectedFileDirectory)
            {
                var dirFiles = _fileSystem.Directory.GetFiles(directoryInfo.FullName);
                (returnList as List<string>).AddRange(dirFiles);
            }

            var subDirectories = _fileSystem.Directory.GetDirectories(directoryInfo.FullName);

            foreach (var directory in subDirectories)
            {
                returnList = returnList.Concat(GetFileNamesFromDirectory(directory, expectedFileDirectory));
            }

            return returnList;
        }

        public long MoveFilesListToDirectory(List<string> fileNames, string destinationDirectoryPath)
        {
            long totalFilesSize = 0;
            foreach (var file in fileNames)
            {
                var fileInfo = _fileSystem.FileInfo.FromFileName(file);
                var newPath = new string(destinationDirectoryPath.Concat(fileInfo.Name).ToArray());
                if (_fileSystem.File.Exists(newPath))
                {
                    newPath = GenerateUniqueFileName(newPath);
                }
                totalFilesSize += fileInfo.Length;
                _fileSystem.File.Move(fileInfo.FullName, newPath);
            }

            return totalFilesSize;
        }
    }
}
