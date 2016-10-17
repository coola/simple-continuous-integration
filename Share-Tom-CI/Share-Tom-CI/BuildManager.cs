using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace Share_Tom_CI
{
    public class BuildManager
    {
        private readonly string _codeFolderPath;
        private readonly string _configuration;
        private readonly string _platform;

        public BuildManager(string codeFolderPath, string configuration, string platform)
        {
            _codeFolderPath = codeFolderPath;
            _configuration = configuration;
            _platform = platform;
        }

        public bool BuildSolution()
        {

            RestorePackages();

            var buildFileUri = CodeManager.GetSolutionFile(_codeFolderPath);

            var props = new Dictionary<string, string>
            {
                ["Configuration"] = _configuration,
                ["Platform"] = _platform
            };
            var request = new BuildRequestData(buildFileUri, props, null, new[] {"Build"}, null);
            var parms = new BuildParameters {Loggers = new List<ILogger> {new ConsoleLogger()}};

            var result = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(parms, request);

            return result.OverallResult == BuildResultCode.Success;
        }

        private void RestorePackages()
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _codeFolderPath,
                    FileName = $@"{AssemblyDirectory()}\nuget.exe",
                    Arguments = "restore",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
        }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine("Output:");
            Console.WriteLine(output);
        }

        private static string AssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}