using System;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Common;
using SimpleContinousIntegration.Builder;
using SimpleContinousIntegration.Maintanance;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class GetTFSCodeTests
    {
        private const string testDebugConfiguration = "Debug";
        private const string testAnyCPUPlatform = "Any CPU";
        private const string testServiceAddress = "https://coola.visualstudio.com/";
        private const string testUserName = "testCoola";
        private const string testPassword = "CoolaHaslo123";
        private const string testProjectFolderPath = "$/CITestProject";

        private readonly string testWorkingDirectoryPath =
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

        private static CodeManager CreateTestProjectCodeManager(string localFolderPath)
        {
            return new CodeManager(GetTestCIConnectionManager().GetTfsTeamProjectCollection(), testProjectFolderPath,
                localFolderPath);
        }

        [Fact]
        public void CheckIfWeHaveConnectionToTFS()
        {
            var connectionManager = GetTestCIConnectionManager();
            var validate = connectionManager.Validate();
            Assert.True(validate);
        }

        public static ConnectionManager GetTestCIConnectionManager()
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

        public CodeManager GetCITestCodeManager()
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

        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testCiConnectionManager = GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir =
                GetCITestCodeManager().GetCode(TestCommits.BuildWrongTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, testDebugConfiguration, testAnyCPUPlatform);
            Assert.False(buildManager.BuildSolution());
        }

        [Fact]
        public void CheckBuild()
        {
            var buildResult = RetrieveCodeAndBuild(null);
            Assert.True(buildResult);
        }

        [Fact]
        public void CheckGettingSolutionFile()
        {
            var codeFolderPath = GetCode();
            var solutionFile = CodeManager.GetSolutionFile(codeFolderPath);
            Assert.False(string.IsNullOrEmpty(solutionFile));
        }

        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeFolderPath = GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        [Fact]
        public void CheckIfGoodCommitIsOK()
        {
            var testCiConnectionManager = GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var resultOfBuild = RetrieveCodeAndBuild(TestCommits.BuildOKTestWrong);
            Assert.True(resultOfBuild);
        }

        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        private bool RetrieveCodeAndBuild(int? changesetID)
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

        [Fact]
        public void RunTestsSuccessfuly()
        {
            var pathToCodeDir = GetCITestCodeManager().GetCode(TestCommits.BuildOKTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, testDebugConfiguration, testAnyCPUPlatform);
            buildManager.BuildSolution();
            Assert.True(new TestManager(testWorkingDirectoryPath, buildManager.CurrentAssemblyList).RunTests());
        }

        [Fact]
        public void CheckCIInterface()
        {
            var ci = CiFactory();
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();
            Assert.True(
                File.Exists(Path.Combine(testWorkingDirectoryPath,
                    new MaintananceManager(testWorkingDirectoryPath).GetNewestDirectory(),
                    ResultsManager._buildOKFileName)));
        }

        [Fact]
        public void TestAssemblyFinderMsBuildManager()
        {
            var pathToCodeDir = GetCITestCodeManager().GetCode();
            var buildManager = new MsBuildBuildManager(pathToCodeDir, testDebugConfiguration, testAnyCPUPlatform);
            buildManager.BuildSolution();
            var buildManagerCurrentAssemblyList = buildManager.CurrentAssemblyList;
            Assert.NotEmpty(buildManagerCurrentAssemblyList);
            Assert.True(buildManagerCurrentAssemblyList.All(a => a.EndsWith(".dll")));
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
            var directorySizeAfterTrim = DirectorySize();          
            Assert.True(maintananceManager.MaxBytesDirectoryCount > directorySizeAfterTrim );
        }

        [Fact]
        public void CheckGettingOFCurrentMsBuildPath()
        {
            var currentMsBuildPath = MsBuildBuildManager.GetCurrentMsBuildPath();
            Assert.False(currentMsBuildPath.IsNullOrEmpty());
        }

        [Fact]
        public void CheckIfItIsTimeToBuild()
        {
            var ci = CiFactory();
            Assert.False(ci.ItIsTimeToBuild());
        }

        private CI CiFactory()
        {
            return new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword,
                testWorkingDirectoryPath,
                TestCommits.BuildOKTestOK, testDebugConfiguration, testAnyCPUPlatform);
        }
    }
}