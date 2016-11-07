using SimpleContinousIntegration.Build;

namespace SimpleContinousIntegration
{
    public class CI
    {
        private readonly string _serviceAddress;
        private readonly string _projectFolderPath;
        private readonly string _userName;
        private readonly string _passWord;
        private readonly string _localBuildFolder;
        private readonly int? _changesetId;
        private readonly string _configuration;
        private readonly string _platform;

        public CI(string serviceAddress, string projectFolderPath, string userName, string passWord,
            string localBuildFolder, int? changesetId, string configuration, string platform)
        {
            _serviceAddress = serviceAddress;
            _projectFolderPath = projectFolderPath;
            _userName = userName;
            _passWord = passWord;
            _localBuildFolder = localBuildFolder;
            _changesetId = changesetId;
            _configuration = configuration;
            _platform = platform;
        }

        private string GetLogText => $"Continous Integration for: {_serviceAddress}{_projectFolderPath}";

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResults()
        {
            LogManager.Log($"Starting {GetLogText}", TextColor.Red);
            var connectionManager = new ConnectionManager(new ConnectionInfo
            {
                ServiceAddress = _serviceAddress,
                UserName = _userName,
                Password = _passWord
            });

            var tfsTeamProjectCollection = connectionManager.GetTfsTeamProjectCollection();

            var codeManager = new CodeManager(tfsTeamProjectCollection, _projectFolderPath, _localBuildFolder);

            var retrievedCodeDirectory = codeManager.GetCode(_changesetId);

            var buildManager = new MsBuildBuildManager(retrievedCodeDirectory, _configuration, _platform);

            var buildSolution = buildManager.BuildSolution();

            var testsManager = new TestManager(retrievedCodeDirectory, buildManager.CurrentAssemblyList);

            var testsRunResult = testsManager.RunTests();

            var resultsManager = new ResultsManager(retrievedCodeDirectory, buildSolution, testsRunResult);

            resultsManager.SaveResults();

            LogManager.Log($"End of {GetLogText}", TextColor.Green);
        }
    }
}