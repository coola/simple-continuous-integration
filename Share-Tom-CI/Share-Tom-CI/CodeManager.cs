using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Share_Tom_CI
{
    public class CodeManager
    {
        public string GetCode()
        {
            var tpc = ConnectionManager.GetTfsTeamProjectCollection();

            tpc.Authenticate();

            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();
            ItemSet itemSet = vcServer.GetItems("$/ShARe-Evolution/ShARe-TOM", RecursionType.Full);
            var latestChangesetId = vcServer.GetLatestChangesetId();

            var dateTime = DateTime.Now;
            var pathDir =
                $@"C:\Data\Source\ShareTomBuildDir_{dateTime.Year}_{dateTime.Month}_{dateTime.Day}-{dateTime.Hour}_{dateTime
                    .Minute}_{dateTime.Second}_ver_{latestChangesetId}";

            Directory.CreateDirectory(pathDir);

            foreach (Item item in itemSet.Items)
            {
                var serverItem = item.ServerItem;
                Debug.WriteLine($"Downloading: {serverItem}");

                var filePath = serverItem.Replace("$/ShARe-Evolution/ShARe-TOM", string.Empty);

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