using System;
using System.IO;

namespace FileManager
{
    internal class FileProcessor
    {
        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "complete";

        public string InputFilePath { get; }

        public FileProcessor(string filePath)
        {
            InputFilePath = filePath;
        }
        public void Process()
        {
            Console.WriteLine($"Begin process of {InputFilePath}");

            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine($"ERROR: file {InputFilePath} does not exist.");
                return;
            }

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            Console.WriteLine($"Root data path is {rootDirectoryPath}");

            // Check if backup dir exists
            string inputFileDirectoryPath = Path.GetDirectoryName(InputFilePath);
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //if (!Directory.Exists(backupDirectoryPath))
            //{
               //Console.WriteLine($"Creating {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            //}

            // Copy file to backup dir
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            Console.WriteLine($"Copying {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);

            // Move to in progress dir
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));

            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);
            if (File.Exists(inProgressFilePath))
            {
                Console.WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed...");
                return;
            }

            Console.WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);

            // Determine type of file
            string extension = Path.GetExtension(InputFilePath);
            switch (extension)
            {
                case ".txt":
                    ProcessTextFile(inProgressFilePath);
                    break;
                default:
                    Console.WriteLine($"{extension} is an unsupported file type.");
                    break;
            }
        }

        private void ProcessTextFile(string inProgressFilePath)
        {
            Console.WriteLine($"Processing text file {inProgressFilePath}");
            // Read in and process

        }
    }
}