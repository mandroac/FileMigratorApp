using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using System.Linq;

namespace FileMigrator.Tests
{
    public class FileMigrationServiceTests
    {
        private const string startDirectory = @"c:\StartDir\";
        private const string moveToDirectory = @"c:\MoveToDir\";
        private const string expectedDirectoryName = "SubDirName";

        private MockFileSystem _mockFileSystem;
        private FileMigrationService _fileMigrationService;

        [Fact]
        public void GetFileNamesFromDirectory_CheckIfReturnsCorrectNumberOfFilesInExpectedDirectories()
        {
            // Arrange
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{startDirectory}{expectedDirectoryName}\\file1.txt", new MockFileData("file1") },
                { $"{startDirectory}{expectedDirectoryName}\\file2.txt" , new MockFileData("file2") },
                { $"{startDirectory}{expectedDirectoryName}\\file3.txt" , new MockFileData("file3") },
                { $"{startDirectory}OtherDirectory\\file4.txt", new MockFileData("file4") },
                { $"{startDirectory}OtherDirectory\\OtherSubdirectory\\file5.txt", new MockFileData("file5") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file6.txt", new MockFileData("file6") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1.txt" , new MockFileData("file7-1") } // ім'я файлу співпадає з іменем файлу в іншій папці
            });

            _fileMigrationService = new FileMigrationService(_mockFileSystem);

            // Act
            var result = _fileMigrationService.GetFileNamesFromDirectory(startDirectory, expectedDirectoryName);
            var returnList = new List<string>(result);

            // Assert
            Assert.Equal(5, returnList.Count);
        }

        [Fact]
        public void GetFileNamesFromDirectory_ReturnsExpectedFilesOnly()
        {
            // Arrange
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{startDirectory}{expectedDirectoryName}\\file1.txt", new MockFileData("file1") },
                { $"{startDirectory}OtherDirectory\\file4.txt", new MockFileData("file4") },
                { $"{startDirectory}OtherDirectory\\OtherSubdirectory\\file5.txt", new MockFileData("file5") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file6.txt", new MockFileData("file6") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1.txt" , new MockFileData("file7-1") }
            });

            _fileMigrationService = new FileMigrationService(_mockFileSystem);

            var expectedFiles = new Dictionary<string, MockFileData>
            {
                { $"{startDirectory}{expectedDirectoryName}\\file1.txt", new MockFileData("file1") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file6.txt", new MockFileData("file6") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1.txt" , new MockFileData("file7-1") }
            };

            // Act
            var result = _fileMigrationService.GetFileNamesFromDirectory(startDirectory, expectedDirectoryName);
            var returnList = new List<string>(result);

            // Assert
            foreach (var file in returnList)
            {
                Assert.Contains(file, expectedFiles.Keys);
            }
        }

        [Fact]
        public void MoveFilesListToDirectory_CalculatesFilesSizeCorrectly()
        {
            // Arrange
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{startDirectory}{expectedDirectoryName}\\file1.txt", new MockFileData("file1") },
                { $"{startDirectory}{expectedDirectoryName}\\file2.txt" , new MockFileData("file2") },
                { $"{startDirectory}{expectedDirectoryName}\\file3.txt" , new MockFileData("file3") },
                { $"{startDirectory}OtherDirectory\\file4.txt", new MockFileData("file4") },
                { $"{startDirectory}OtherDirectory\\OtherSubdirectory\\file5.txt", new MockFileData("file5") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file6.txt", new MockFileData("file6") },
                { $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1.txt" , new MockFileData("file7=file1") }
            });
            _mockFileSystem.AddDirectory(moveToDirectory);

            _fileMigrationService = new FileMigrationService(_mockFileSystem);

            var fileNames = new List<string>(_mockFileSystem.AllFiles.Where(f => f.Contains(expectedDirectoryName)));
            long expectedSize = 0;
            fileNames.ForEach(fileName =>
            {
                expectedSize += _mockFileSystem.GetFile(fileName).Contents.Length;
            });

            // Act
            var result = _fileMigrationService.MoveFilesListToDirectory(fileNames, moveToDirectory);

            // Assert
            Assert.Equal(expectedSize, result);
        }

        [Fact]
        public void GenerateUniqueFileName_GeneratesNewFileNameCorrectly()
        {
            // Arrange
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{startDirectory}{expectedDirectoryName}\\file1.txt", new MockFileData("file1") }
            });

            _fileMigrationService = new FileMigrationService(_mockFileSystem);
            string expectedName = $"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1 (1).txt";

            // Act 
            var result = _fileMigrationService.GenerateUniqueFileName($"{startDirectory}OtherDirectory\\{expectedDirectoryName}\\file1.txt");

            // Assert 
            Assert.Equal(expectedName, result);
        }
    }
}

