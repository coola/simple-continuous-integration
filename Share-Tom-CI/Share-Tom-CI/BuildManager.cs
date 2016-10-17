using System.Collections.Generic;
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

        public BuildManager(string codeFolderPath, string configuration, string platform )
        {
            _codeFolderPath = codeFolderPath;
            _configuration = configuration;
            _platform = platform;
        }

        public bool BuildSolution()
        {
            var buildFileUri = CodeManager.GetSolutionFile(_codeFolderPath);
           
            var props = new Dictionary<string, string>
            {
                ["Configuration"] = _configuration,
                ["Platform"] = _platform
            };
            var request = new BuildRequestData(buildFileUri, props, null, new[] { "Build" }, null);
            var parms = new BuildParameters { Loggers = new List<ILogger> { new ConsoleLogger() } };

            var result = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(parms, request);
            return result.OverallResult == BuildResultCode.Success;
        }
    }
}