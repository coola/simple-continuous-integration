﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using SimpleContinousIntegration.Connection;
using SimpleContinousIntegration.Log;
using SimpleContinousIntegration.Maintanance;

namespace SimpleContinousIntegration.BuildFolder
{
    public class BuildFolderManager
    {
        private readonly string _projectFolderPath;
        private readonly string _localFolderPath;
        private VersionControlServer _versionControlService;
        private readonly string _buildFolderPrefix;
        private TfsTeamProjectCollection _tfsTeamProjectCollection;

        public BuildFolderManager(string serviceAddress, string userName, string passWord,
            string remoteProjectFolderPath, string localBuildFolder) : this(remoteProjectFolderPath, localBuildFolder)
        {
            var connectionManager = new ConnectionManager(serviceAddress, userName, passWord);
            ConstructVersionControlService(connectionManager.GetTfsTeamProjectCollection());
        }

        public BuildFolderManager(TfsTeamProjectCollection teamProjectCollection, string remoteProjectFolderPath,
            string localFolderPath) : this(remoteProjectFolderPath, localFolderPath)
        {
            ConstructVersionControlService(teamProjectCollection);
        }

        public void ConstructVersionControlService(TfsTeamProjectCollection teamProjectCollection)
        {
            _tfsTeamProjectCollection = teamProjectCollection;
            _versionControlService = _tfsTeamProjectCollection.GetService<VersionControlServer>();
        }

        private BuildFolderManager(string projectFolderPath,
            string localFolderPath)
        {
            _projectFolderPath = projectFolderPath;
            _localFolderPath = localFolderPath;

            if (_localFolderPath.IsNullOrEmpty())
            {
                throw new ArgumentException("You should provide specific temporary folder for builds");
            }

            _localFolderPath = Path.GetFullPath(_localFolderPath);

            new MaintananceManager(_localFolderPath).TrimBuildDirectoryToMaxSize();

            _buildFolderPrefix = RemoveUnnecessarySignsFromRemoteProjectFolderPath(_projectFolderPath);
        }

        public static string RemoveUnnecessarySignsFromRemoteProjectFolderPath(string remoteProjectFolderPath)
        {
            return remoteProjectFolderPath.Replace("$", string.Empty).Replace("/", "_");
        }

        public string GetChangsetAuthor(int ChangesetId)
        {
            return _versionControlService.GetChangeset(ChangesetId).OwnerDisplayName;
        }

        public int? GetMaxCurrentLocalChangeset()
        {
            var allFoldersOrderedByName = new MaintananceManager(_localFolderPath).GetAllFoldersOrderedByName();
            if (!allFoldersOrderedByName.Any()) return null;
            var onlyFromProject = allFoldersOrderedByName.Where(a => a.StartsWith(_buildFolderPrefix)).ToList();
            if (!onlyFromProject.Any()) return null;
            return onlyFromProject
                .Select(a => int.Parse(a.Split('_').Last()))
                .Max();
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
                changesetId = GetLatestChangesetId();
                itemSet = _versionControlService.GetItems(_projectFolderPath, RecursionType.Full);
            }

            var dateTime = DateTime.Now;
            var pathDir = CreateBuildFolderName(dateTime, changesetId);

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

        private string CreateBuildFolderName(DateTime dateTime, int? changesetId)
        {
            var uniqueGuid = Guid.NewGuid().ToString();
            var shortenUniqueGuid = uniqueGuid.Substring(uniqueGuid.Length - 4);
            return Path.Combine($"{_localFolderPath}",
                $@"{_buildFolderPrefix}_{dateTime.Year}_{dateTime.Month:D2}_{dateTime.Day:D2}-{dateTime.Hour:D2}_{dateTime
                    .Minute:D2}_{dateTime.Second:D2}_{shortenUniqueGuid}_ver_{changesetId}");
        }

        public int GetLatestChangesetId()
        {
            return _versionControlService.GetLatestChangesetId();
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