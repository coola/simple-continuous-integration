using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace SimpleContinousIntegration
{
    public class CodeManager
    {
        private readonly string _projectFolderPath;
        private readonly string _localFolderPath;
        private readonly VersionControlServer _versionControlService;
        private const string buildFolderPrefix = "build";

        public CodeManager(TfsConnection teamProjectCollection, string projectFolderPath, string localFolderPath)
        {
            _projectFolderPath = projectFolderPath;

            if (localFolderPath.IsNullOrEmpty())
            {
                throw new ArgumentException("You should provide specific temporary folder for builds");
            }
            
            _localFolderPath = Path.GetFullPath(localFolderPath);
           
            _versionControlService = teamProjectCollection.GetService<VersionControlServer>();
        }

        public string GetChangsetAuthor(int ChangesetId)
        {
            return _versionControlService.GetChangeset(ChangesetId).OwnerDisplayName;
        }

        public string GetCode(int? versionNumber = null)
        {
            LogManager.Log("Getting code", TextColor.Red);
            int? changesetId;
            ItemSet itemSet;
            if (versionNumber != null)
            {
                changesetId = versionNumber;
                itemSet = _versionControlService.GetItems(_projectFolderPath,
                    new ChangesetVersionSpec(versionNumber.Value), RecursionType.Full);
            }
            else
            {
                changesetId = _versionControlService.GetLatestChangesetId();
                itemSet = _versionControlService.GetItems(_projectFolderPath, RecursionType.Full);
            }

            var dateTime = DateTime.Now;
            var pathDir =
                $@"{_localFolderPath}\{buildFolderPrefix}_{dateTime.Year}_{dateTime.Month:D2}_{dateTime.Day:D2}-{dateTime.Hour:D2}_{dateTime
                    .Minute:D2}_{dateTime.Second:D2}_ver_{changesetId}";

            Directory.CreateDirectory(pathDir);

            foreach (var item in itemSet.Items)
            {
                var serverItem = item.ServerItem;
                LogManager.Log($"Downloading: {serverItem}", TextColor.Blue);

                var filePath = serverItem.Replace(_projectFolderPath, string.Empty);

                if (filePath.IsNullOrEmpty()) continue;

                var fullPath = pathDir + filePath;

                if (new FileInfo(fullPath).Exists) continue;

                switch (item.ItemType)
                {
                    case ItemType.File:
                        using (var output = new FileStream(fullPath, FileMode.Create))
                        {
                            item.DownloadFile().CopyTo(output);
                        }
                        break;
                    case ItemType.Folder:
                        Directory.CreateDirectory(fullPath);
                        break;
                    case ItemType.Any:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            LogManager.Log("End of getting code", TextColor.Green);
            LogManager.Log($"Code directory is {pathDir}");

            return pathDir;
        }

        public static string GetSolutionFile(string directoryPath)
        {
            var fileEntries = Directory.GetFiles(directoryPath);
            return fileEntries.Single(file => file.Contains(".sln"));
        }

        public static string AssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}