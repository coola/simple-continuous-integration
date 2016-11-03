using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Coola.VisualStudioServices.SimpleContinousIntegration
{
    public class CodeManager
    {
        private readonly string _projectFolderPath;
        private readonly string _localFolderPath;
        private readonly VersionControlServer _versionControlService;

        public CodeManager(TfsConnection teamProjectCollection, string projectFolderPath,
            string localFolderPath)
        {
            _projectFolderPath = projectFolderPath;
            _localFolderPath = localFolderPath;
            teamProjectCollection.Authenticate();
            _versionControlService = teamProjectCollection.GetService<VersionControlServer>();
        }

        public string GetChangsetAuthor(int ChangesetId)
        {
            return _versionControlService.GetChangeset(ChangesetId).OwnerDisplayName;
        }

        public string GetCode(int? versionNumber = null)
        {
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
                $@"{_localFolderPath}\ShareTomBuildDir_{dateTime.Year}_{dateTime.Month}_{dateTime.Day}-{dateTime.Hour}_{dateTime
                    .Minute}_{dateTime.Second}_ver_{changesetId}";

            Directory.CreateDirectory(pathDir);

            foreach (var item in itemSet.Items)
            {
                var serverItem = item.ServerItem;
                Debug.WriteLine($"Downloading: {serverItem}");

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

            return pathDir;
        }

        public static string GetSolutionFile(string directoryPath)
        {
            var fileEntries = Directory.GetFiles(directoryPath);
            return fileEntries.Single(file => file.Contains(".sln"));
        }
    }
}