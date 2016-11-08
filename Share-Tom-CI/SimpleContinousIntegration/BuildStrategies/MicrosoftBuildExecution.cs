using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using SimpleContinousIntegration.Log;
using SimpleContinousIntegration.RestorePackages;

namespace SimpleContinousIntegration.BuildStrategies
{
    public class MicrosoftBuildExecution : IBuilder
    {
        private const string defaultTarget = "Build";
        private readonly string _codeFolderPath;
        private readonly string _configuration;
        private readonly string _platform;

        public List<string> CurrentAssemblyList { get; private set; }

        public MicrosoftBuildExecution(string codeFolderPath, string configuration, string platform)
        {
            _codeFolderPath = codeFolderPath;
            _configuration = configuration;
            _platform = platform;
        }

        public bool BuildSolution()
        {
            LogManager.Log("Building code", TextColor.Red);

            new RestorePackagesManager(_codeFolderPath).RestorePackages();

            var buildFileUri = BuildFolder.BuildFolderManager.GetSolutionFile(_codeFolderPath);

            var props = new Dictionary<string, string>
            {
                ["Configuration"] = _configuration,
                ["Platform"] = _platform
            };
            var request = new BuildRequestData(buildFileUri, props, null, new[] {defaultTarget}, null);
            var parameters = new BuildParameters {Loggers = new List<ILogger> {new ConsoleLogger()}};

            var result = new BuildManager("localhost").Build(parameters, request);

            //var result = BuildManager.DefaultBuildManager.Build(parameters, request);

            LogManager.Log("End of building code", TextColor.Green);

            PopulateAssemblyListAfterBuild(result);

            return result.OverallResult == BuildResultCode.Success;
        }

        private void PopulateAssemblyListAfterBuild(BuildResult result)
        {
            var taskItems = result.ResultsByTarget[defaultTarget].Items;

            CurrentAssemblyList = taskItems.Select(a => a.ToString()).ToList();
        }
    }
}