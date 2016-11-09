using System;
using SimpleContinousIntegration.BuildFolder;
using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Connection;
using SimpleContinousIntegration.Maintanance;

namespace SimpleContinousIntegration.Tests
{
    public class TestUtilities
    {
         public const string testDebugConfiguration = "Debug";
        public const string testAnyCPUPlatform = "Any CPU";
        public const string testServiceAddress = "https://coola.visualstudio.com/";
        public const string testUserName = "testCoola";
        public const string testPassword = "CoolaHaslo123";
        public const string testProjectFolderPath = "$/CITestProject";

        public readonly string testWorkingDirectoryPath =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Builds\";
       
        public static class TestCommits
        {
            public static int BuildWrongTestOK = 10;
            public static int BuildOKTestWrong = 11;
            public static int BuildOKTestOK = 12;
        }

        public ConnectionManager GetTestCIConnectionManager()
        {
            return new ConnectionManager(GetTestCIConnectionInfo());
        }

        private static ConnectionInfo GetTestCIConnectionInfo()
        {
            return new ConnectionInfo
            {
                ServiceAddress = testServiceAddress,
                UserName = testUserName,
                Password = testPassword
            };
        }

        public BuildFolderManager GetCITestCodeManager()
        {
            return CreateTestProjectCodeManager(testWorkingDirectoryPath);
        }

         public BuildFolderManager CreateTestProjectCodeManager(string localFolderPath)
        {
            return new BuildFolderManager(GetTestCIConnectionManager().GetTfsTeamProjectCollection(), testProjectFolderPath,
                localFolderPath);
        }


        public string GetCode()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            return codeFolderPath;
        }

        
        public bool RetrieveCodeAndBuild(int? changesetID)
        {
            var buildManager = RetrieveCodeAndBuildReturnBuilder(changesetID);
            return buildManager.BuildSolution();
        }

        private IBuilder RetrieveCodeAndBuildReturnBuilder(int? changesetID)
        {
            var pathToCodeDir = GetCITestCodeManager().GetCode(changesetID);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, testDebugConfiguration, testAnyCPUPlatform);
            return buildManager;
        }

        public long DirectorySize()
        {
            var maintananceManager = GetMaintananceManager();
            var directorySize = maintananceManager.GetDirectorySize();
            return directorySize;
        }

        public MaintananceManager GetMaintananceManager()
        {
            return new MaintananceManager(testWorkingDirectoryPath);
        }

         public CI CiFactoryBuildOKTestsOK()
        {
            return new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword,
                testWorkingDirectoryPath,
                TestCommits.BuildOKTestOK, testDebugConfiguration, testAnyCPUPlatform);
        }
        public CI CiFactoryNewest()
        {
            return new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword,
                testWorkingDirectoryPath,
                null, testDebugConfiguration, testAnyCPUPlatform);
        }
    }
}
