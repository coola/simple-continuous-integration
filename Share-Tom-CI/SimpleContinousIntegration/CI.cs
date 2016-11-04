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

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResults()
        {
            var connectionManager = new ConnectionManager(new ConnectionInfo
            {
                ServiceAddress = _serviceAddress,
                UserName = _userName,
                Password = _passWord
            });

            var tfsTeamProjectCollection = connectionManager.GetTfsTeamProjectCollection();

            var codeManager = new CodeManager(tfsTeamProjectCollection, _projectFolderPath, _localBuildFolder);

            var retrievedCodeDirectory = codeManager.GetCode(_changesetId);

            var buildManager = new BuildManager(retrievedCodeDirectory, _configuration, _platform);

            var buildSolution = buildManager.BuildSolution();

            var testsManager = new TestManager(retrievedCodeDirectory);

            var testsRunResult = testsManager.RunTests();

            var resultsManager = new ResultsManager(retrievedCodeDirectory, buildSolution, testsRunResult);

            resultsManager.SaveResults();
        }
    }
}