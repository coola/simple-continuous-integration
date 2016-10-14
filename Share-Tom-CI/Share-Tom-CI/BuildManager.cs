using System.Collections.Generic;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;

namespace Share_Tom_CI
{
    public class BuildManager
    {
        private readonly string _codeFolderPath;

        public BuildManager(string codeFolderPath)
        {
            _codeFolderPath = codeFolderPath;
        }

        public bool BuildSolution()
        {
            var buildFileUri = CodeManager.GetSolutionFile(_codeFolderPath);

            var props = new Dictionary<string, string>
            {
                ["Configuration"] = "DebugDefaultModule",
                ["Platform"] = "x64"
            };
            var request = new BuildRequestData(buildFileUri, props, null, new [] { "Clean" }, null);
            var parms = new BuildParameters {Loggers = new[] {new FileLogger()}};

            var result = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(parms, request);
            return result.OverallResult == BuildResultCode.Success;
        }
    }
}