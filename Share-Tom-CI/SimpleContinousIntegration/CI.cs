using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Connection;
using SimpleContinousIntegration.Log;
using SimpleContinousIntegration.Results;
using SimpleContinousIntegration.Test;

namespace SimpleContinousIntegration
{
    public class CI
    {
        private readonly string _serviceAddress;
        private readonly string _remoteProjectFolderPath;
        private readonly string _userName;
        private readonly string _passWord;
        private readonly string _localBuildFolder;
        private int? _changesetId;
        private readonly string _configuration;
        private readonly string _platform;
        private readonly BuildFolder.BuildFolder _buildFolder;
        private int currentWaitPeriod = 2;

        public CI(string serviceAddress, string remoteProjectFolderPath, string userName, string passWord,
            string localBuildFolder, int? changesetId, string configuration, string platform)
        {
            _serviceAddress = serviceAddress;
            _remoteProjectFolderPath = remoteProjectFolderPath;
            _userName = userName;
            _passWord = passWord;
            _localBuildFolder = localBuildFolder;
            _changesetId = changesetId;
            _configuration = configuration;
            _platform = platform;
            _buildFolder = GetCodeManager();
        }

        private string GetLogText => $"Continous Integration for: {_serviceAddress}{_remoteProjectFolderPath}";

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResultsContinous()
        {
            if (ItIsTimeToBuild())
            {
                _changesetId = _buildFolder.GetMaxCurrentLocalChangeset() + 1;

                RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();

                ResetWaitPeriod();
            }
            else
            {
                LogManager.Log("Skiping as it is no new changset in comparison to local build folder.");
                //LogManager.Log($"Sleeping {currentWaitPeriod} second(s). Press esc to stop.");
                //Thread.Sleep(currentWaitPeriod * 1000);
                //IncreaseWaitPeriod();
            }
        }

        private void ResetWaitPeriod()
        {
            currentWaitPeriod = 2;
        }

        private void IncreaseWaitPeriod()
        {
            currentWaitPeriod = currentWaitPeriod < 3600 ? currentWaitPeriod * 2 : currentWaitPeriod;
        }

        public bool ItIsTimeToBuild()
        {
            var maxCurrentLocalChangeset = _buildFolder.GetMaxCurrentLocalChangeset();
            var latestChangesetId = _buildFolder.GetLatestChangesetId();
            return maxCurrentLocalChangeset < latestChangesetId;
        }

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce()
        {
            LogManager.Log($"Starting {GetLogText}", TextColor.Red);

            var codeManager = GetCodeManager();

            var retrievedCodeDirectory = codeManager.GetCode(_changesetId);

            var buildManager = new MsBuildBuildManager(retrievedCodeDirectory, _configuration, _platform);

            var buildSolution = buildManager.BuildSolution();

            var testsManager = new TestManager(retrievedCodeDirectory, buildManager.CurrentAssemblyList);

            var testsRunResult = testsManager.RunTests();

            var resultsManager = new ResultsManager(retrievedCodeDirectory, buildSolution, testsRunResult);

            resultsManager.SaveResults();

            LogManager.Log($"End of {GetLogText}", TextColor.Green);
        }

        private BuildFolder.BuildFolder GetCodeManager()
        {
            var connectionManager = new ConnectionManager(new ConnectionInfo
            {
                ServiceAddress = _serviceAddress,
                UserName = _userName,
                Password = _passWord
            });

            var tfsTeamProjectCollection = connectionManager.GetTfsTeamProjectCollection();

            var codeManager = new BuildFolder.BuildFolder(tfsTeamProjectCollection, _remoteProjectFolderPath, _localBuildFolder);
            return codeManager;
        }
    }
}