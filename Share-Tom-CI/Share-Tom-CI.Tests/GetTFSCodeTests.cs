using System.IO;
using System.Linq;
using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests
    {
        [Fact]
        public void CheckIfWeHaveWorkingFolder()
        {
            var localWorkingDirectoryPath = GetLocalWorkingDirectoryPath();
            Directory.CreateDirectory(localWorkingDirectoryPath);
            Directory.Exists(localWorkingDirectoryPath);
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
                ServiceAddress = "https://coola.visualstudio.com/",
                UserName = "testCoola",
                Password = "CoolaHaslo123"
            };
        }

        public static CodeManager GetCITestCodeManager()
        {
            return new CodeManager(GetTestCIConnectionManager().GetTfsTeamProjectCollection(), "$/CITestProject",
                GetLocalWorkingDirectoryPath());
        }

        private static string GetLocalWorkingDirectoryPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + @"\Data\Source\";
        }

        private static string GetNewestDirectory()
        {
            var directory = new DirectoryInfo(GetLocalWorkingDirectoryPath());
            return (from f in directory.GetFiles()
                orderby f.LastWriteTime descending
                select f.Name).First();
        }

        public static string GetCode()
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

        public static class TestCommits
        {
            public static int BuildWrongTestOK = 10;
            public static int BuildOKTestWrong = 11;
            public static int BuildOKTestOK = 12;
        }

        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testCiConnectionManager = GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir =
                GetCITestCodeManager().GetCode(TestCommits.BuildWrongTestOK);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.False(buildManager.BuildSolution());
        }


        [Fact]
        public void CheckBuild()
        {
            var codeFolderPath = GetCode();
            var buildManager = new BuildManager(codeFolderPath, "Debug", "Any CPU");
            var buildResult = buildManager.BuildSolution();
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
            var pathToCodeDir =
                GetCITestCodeManager().GetCode(TestCommits.BuildOKTestWrong);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.True(buildManager.BuildSolution());
        }

        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}