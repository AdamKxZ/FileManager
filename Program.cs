using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing command lind options");

            // Command line validation omitted
            
            var commands = args[0];

            if (commands == "--file")
            {
                var filePath = args[1];
                Console.WriteLine($"Single file {filePath} selected");
                ProcessSingleFile(filePath);
            }
            else if (commands == "--dir")
            {
                var directoryPath = args[1];
                var fileType = args[2];
                Console.WriteLine($"Directory {directoryPath} selected for {fileType} files");
                ProcessDirectory(directoryPath, fileType);
            }
            else
            {
                Console.WriteLine("Invalid command line options");
            }

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();

        }
        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }
        private static void ProcessDirectory(string directoryPath, string fileType)
        {

        }
    }
}
