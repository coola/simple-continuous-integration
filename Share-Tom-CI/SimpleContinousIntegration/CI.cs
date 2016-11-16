using System;
using System.Threading;
using SimpleContinousIntegration.BuildFolder;
using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Log;
using SimpleContinousIntegration.Results;
using SimpleContinousIntegration.Test;
using SimpleContinousIntegration.Time;

namespace SimpleContinousIntegration
{
    /// <summary>
    /// Class that allows you to connect to Visual Studio Service to TFS service
    /// It dowload current code 
    /// It builds it.
    /// It run tests.
    /// It produces output of build and run.
    /// </summary>
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
        private readonly BuildFolderManager _buildFolderManager;
        private readonly TimeManager _timeManager;
        

        /// <summary>
        /// Constructor for customizing every parameter
        /// </summary>
        /// <param name="serviceAddress"></param>
        /// <param name="remoteProjectFolderPath"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="localBuildFolder"></param>
        /// <param name="changesetId"></param>
        /// <param name="configuration"></param>
        /// <param name="platform"></param>
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
            _buildFolderManager = new BuildFolderManager(_serviceAddress, _userName, _passWord, _remoteProjectFolderPath, _localBuildFolder);
            _timeManager = new TimeManager(_buildFolderManager);
        }

        private string GetLogText => $"Continous Integration for: {_serviceAddress}{_remoteProjectFolderPath}";

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResultsContinous()
        {
            WriteStartLog();

            while (true)
            {
                if (_timeManager.ItIsTimeToBuild())
                {
                    var maxCurrentLocalChangeset = _buildFolderManager.GetMaxCurrentLocalChangeset();
                    if (maxCurrentLocalChangeset != null)
                    {
                        _changesetId = maxCurrentLocalChangeset + 1;
                    }

                    RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();

                    _timeManager.ResetWaitPeriod();
                }
                else
                {
                    var currentWaitPeriod = _timeManager.CurrentWaitPeriod;
                    LogManager.Log("Skiping as it is no new changset in comparison to local build folder.");
                    LogManager.Log($"Sleeping {currentWaitPeriod} second(s). Press esc to stop.");
                    LogManager.Log($"Next repository check at {DateTime.Now.AddSeconds(currentWaitPeriod):hh:mm:ss}");
                    Thread.Sleep(currentWaitPeriod * 1000);
                    _timeManager.IncreaseWaitPeriod();
                }
            }
        }

        private void WriteStartLog()
        {
            LogManager.Log("Starting Simple Continous Integration Server for Microsoft Visual Services for:", TextColor.Blue);
            LogManager.Log($"Service address:             {_serviceAddress}", TextColor.Blue);
            LogManager.Log($"Remote project folder path:  {_remoteProjectFolderPath}", TextColor.Blue);
            LogManager.Log($"Local build folder is:       {_localBuildFolder}", TextColor.Blue);
        }
        
        public void RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce()
        {
            LogManager.Log($"Starting {GetLogText}", TextColor.Red);

            var retrievedCodeDirectory = _buildFolderManager.GetCode(_changesetId);

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