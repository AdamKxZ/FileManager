using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.Caching;
using System.Threading;

namespace FileManager
{
    class Program
    {

        private static MemoryCache FilesToProcess = MemoryCache.Default;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing command lind options");

            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                Console.WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                Console.WriteLine($"Watching directory {directoryToWatch} for changes");

                ProcessExistingFiles(directoryToWatch);

                using (var inputFileWatch = new FileSystemWatcher(directoryToWatch))
                {
                    inputFileWatch.IncludeSubdirectories = false;  
                    inputFileWatch.InternalBufferSize = 32768; // 32KB
                    inputFileWatch.Filter = "*.*";
                    inputFileWatch.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                    inputFileWatch.Created += FileCreated;
                    inputFileWatch.Changed += FileChanged;
                    inputFileWatch.Deleted += FileDeleted;
                    inputFileWatch.Renamed += FileRenamed;
                    inputFileWatch.Error += WatcherError;

                    inputFileWatch.EnableRaisingEvents = true;

                    Console.WriteLine("Press enter to quit.");
                    Console.ReadLine();

                }
            }
        }
        private static void ProcessExistingFiles(string inputDirectory)
        {
            Console.WriteLine($"Checking {inputDirectory} for existing files");

            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                Console.WriteLine($"  - Found {filePath}");
                AddToCache(filePath);
            }
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File created {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
         }
        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File changed {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
        }
        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File deleted {e.Name} - type: {e.ChangeType}");
        }
        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File renamed: {e.OldName} to {e.Name}- type: {e.ChangeType}");
        }
        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"ERROR: file system watching may no longer be actice: {e.GetException()}");
        }

        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);

            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2),
            };
            FilesToProcess.Add(item, policy);
        }

        private static void ProcessFile(CacheEntryRemovedArguments args)
        {
            Console.WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}");

            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(args.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                Console.WriteLine($"WARNING: {args.CacheItem.Key} was removed unexpectedly and may not be used.");
            }    
        }


        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }
        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            // var allFiles = Directory.GetFiles(directoryPath); // to get all files

            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    Console.WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }

        /*private static void ProcessFiles(Object stateInfo)
        {
            foreach (var fileName in FilesToProcess.Keys) // May not be in order of adding
            {
                if (FilesToProcess.TryRemove(fileName, out _))
                {
                    var fileProcessor = new FileProcessor(fileName);
                    fileProcessor.Process();
                }
            }    
        }*/
    }
}
