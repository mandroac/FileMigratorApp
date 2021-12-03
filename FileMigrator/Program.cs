using System;
using System.IO.Abstractions;
using System.Linq;

namespace FileMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPath = @"D:\Studying\Self-studying\FileMigratorApp\DIR\";
            string moveToDirPath = @"D:\Studying\Self-studying\FileMigratorApp\DIR_TO_MOVE_TO\";
            string expectedDirName = "SUB_DIR_NAME";

            var fileMigrationService = new FileMigrationService(new FileSystem());

            var fileNamesList = fileMigrationService.GetFileNamesFromDirectory(dirPath, expectedDirName).ToList();
            if (fileNamesList.Count == 0)
            {
                Console.WriteLine($"No files in '{expectedDirName}' subdirectories were found.");
            }
            else
            {
                Console.WriteLine($"Found {fileNamesList.Count} files in '{expectedDirName}' subdirectories: \n\n");
                foreach (var file in fileNamesList)
                {
                    Console.WriteLine(file);
                }
                var totalFilesSize = fileMigrationService.MoveFilesListToDirectory(fileNamesList, moveToDirPath);


                Console.WriteLine($"\n\n All files were moved to {moveToDirPath} successfully. Total size is {totalFilesSize} bytes.");
            }
        }
    }
}
