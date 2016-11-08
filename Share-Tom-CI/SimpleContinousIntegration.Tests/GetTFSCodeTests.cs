using System;
using System.IO;
using Microsoft.TeamFoundation.Common;
using SimpleContinousIntegration.BuildFolder;
using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Connection;
using SimpleContinousIntegration.Maintanance;
using SimpleContinousIntegration.Results;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class GetTFSCodeTests
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

        public GetTFSCodeTests()
        {
            Directory.CreateDirectory(testWorkingDirectoryPath);
        }

        [Fact]
        public void CheckIfWeHaveWorkingFolder()
        {
            var localWorkingDirectoryPath = testWorkingDirectoryPath;
            Directory.CreateDirectory(localWorkingDirectoryPath);
            Directory.Exists(localWorkingDirectoryPath);
        }

        [Fact]
        public void CheckCodeManagerForEmptyFolder()
        {
            Assert.Throws<ArgumentException>(() => CreateTestProjectCodeManager(string.Empty));
        }

        private BuildFolderManager CreateTestProjectCodeManager(string localFolderPath)
        {
            return new BuildFolderManager(GetTestCIConnectionManager().GetTfsTeamProjectCollection(), testProjectFolderPath,
                localFolderPath);
        }

        [Fact]
        public void CheckIfWeHaveConnectionToTFS()
        {
            var connectionManager = GetTestCIConnectionManager();
            var validate = connectionManager.Validate();
            Assert.True(validate);
        }

        public ConnectionManager GetTestCIConnectionManager()
        {
            return new ConnectionManager(GetTestCIConnectionInfo());
        }

        private ConnectionInfo GetTestCIConnectionInfo()
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

        public string GetCode()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            return codeFolderPath;
        }

        [Fact]
        public void SendBrokenBuildMail()
        {
            var tfsTeamProjectCollection = GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            var ciTestCodeManager = GetCITestCodeManager();
            var changsetAuthor = ciTestCodeManager.GetChangsetAuthor(TestCommits.BuildWrongTestOK);
            new MailManager(TestCommits.BuildWrongTestOK, changsetAuthor).SendNoBuildMessageUsingVisualStudioServices(
                tfsTeamProjectCollection);
        }

        [Fact]
        public void SendBrokenBuildMailConcreteUser()
        {
            var tfsTeamProjectCollection = GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            new MailManager(TestCommits.BuildWrongTestOK, "Michał Kuliński").SendNoBuildMessageUsingVisualStudioServices
                (tfsTeamProjectCollection);
        }

        [Fact]
        public void CheckOwner()
        {
            var ciTestCodeManager = GetCITestCodeManager();
            var changsetAuthor = ciTestCodeManager.GetChangsetAuthor(TestCommits.BuildWrongTestOK);
            Assert.Equal("Coola", changsetAuthor);
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

         private long DirectorySize()
        {
            var maintananceManager = GetMaintananceManager();
            var directorySize = maintananceManager.GetDirectorySize();
            return directorySize;
        }

        private MaintananceManager GetMaintananceManager()
        {
            return new MaintananceManager(testWorkingDirectoryPath);
        }

        [Fact]
        public void CheckDirectorySizeCounter()
        {
            var directorySize = DirectorySize();
            Assert.NotEqual(0, directorySize);
        }

        [Fact]
        public void CheckDirectorySizeCounterTrimFunctionality()
        {
            var maintananceManager = GetMaintananceManager();
            maintananceManager.TrimBuildDirectoryToMaxSize();
        }

        [Fact]
        public void CheckGettingOFCurrentMsBuildPath()
        {
            var currentMsBuildPath = MsBuildBuildManager.GetCurrentMsBuildPath();
            Assert.False(currentMsBuildPath.IsNullOrEmpty());
        }

        public CI CiFactory()
        {
            return new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword,
                testWorkingDirectoryPath,
                TestCommits.BuildOKTestOK, testDebugConfiguration, testAnyCPUPlatform);
        }
    }
}