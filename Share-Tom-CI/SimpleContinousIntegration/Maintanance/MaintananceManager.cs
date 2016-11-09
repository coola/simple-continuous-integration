using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleContinousIntegration.Maintanance
{
    public class MaintananceManager
    {
        private readonly string _buildFolderPath;

        public long MaxBytesDirectoryCount { get; } = _10GigaBytesInBytes;

        private static long _10GigaBytesInBytes => 10000000000;

        public MaintananceManager(string buildFolderPath)
        {
            _buildFolderPath = buildFolderPath;
            Directory.CreateDirectory(_buildFolderPath);
        }

        public MaintananceManager(string buildFolderPath, long maxBytesDirectoryCount) : this (buildFolderPath)
        {
            MaxBytesDirectoryCount = maxBytesDirectoryCount;
        }

        public void TrimBuildDirectoryToMaxSize()
        {
            var currentSize = GetDirectorySize();
            while (currentSize > MaxBytesDirectoryCount)
            {
                DeleteDirectory(Path.Combine(_buildFolderPath, GetOldestDirectory()));
                currentSize = GetDirectorySize();
            }
        }

        private static void DeleteDirectory(string preparedToDeletionDirectoryPath)
        {
            var directoryInfo = new DirectoryInfo(preparedToDeletionDirectoryPath);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }

            directoryInfo.Delete();
        }
        

        public long GetDirectorySize()
        {
            var di = new DirectoryInfo(_buildFolderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        public string GetNewestDirectory()
        {
            return GetAllFoldersOrderedByName().First();
        }

        private string GetOldestDirectory()
        {
            return GetAllFoldersOrderedByName().Last();
        }

        public void RemoveAllEmpty()
        {
            var allFoldersByWriteTime = GetAllFoldersOrderedByName();
            foreach (var folder in allFoldersByWriteTime)
            {
                var directoryInfo = new DirectoryInfo(Path.Combine(_buildFolderPath, folder));

                if (directoryInfo.GetDirectories().Length != 0 || directoryInfo.GetFiles().Length != 0) continue;
                try
                {
                    directoryInfo.Delete();
                }
                catch (IOException exception)
                {
                    if (!exception.Message.Contains("The process cannot access the file"))
                    {
                        throw;
                    }
                }
            }
        }

        public List<string> GetAllFoldersOrderedByName()
        {
            var directory = new DirectoryInfo(_buildFolderPath);
            return (from f in directory.GetDirectories()
                orderby f.Name descending
                select f.Name).ToList();
        }
    }
}