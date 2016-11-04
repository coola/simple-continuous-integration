using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace SimpleContinousIntegration
{
    public class BuildManager
    {
        private const string defaultTarget = "Build";
        private readonly string _codeFolderPath;
        private readonly string _configuration;
        private readonly string _platform;

        public List<string> CurrentAssemblyList { get; private set; }

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
            var request = new BuildRequestData(buildFileUri, props, null, new[] {defaultTarget}, null);
            var parms = new BuildParameters {Loggers = new List<ILogger> {new ConsoleLogger()}};

            var result = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(parms, request);

            PopulateAssemblyListAfterBuild(result);

            return result.OverallResult == BuildResultCode.Success;
        }

        private void PopulateAssemblyListAfterBuild(BuildResult result)
        {
            var taskItems = result.ResultsByTarget[defaultTarget].Items;

            CurrentAssemblyList = taskItems.Select(a => a.ToString()).ToList();
        }

        private void RestorePackages()
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _codeFolderPath,
                    FileName = $@"{CodeManager.AssemblyDirectory()}\nuget.exe",
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
    }
}