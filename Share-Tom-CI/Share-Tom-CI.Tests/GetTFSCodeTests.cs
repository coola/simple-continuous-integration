using System.IO;
using System.Linq;
using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests
    {

        [Fact]
        public void CheckIfWeHaveConnectionToTFS()
        {
            var connectionManager = GetKerringConnectionManager();
            var validate = connectionManager.Validate();
            Assert.True(validate);
        }

        private static ConnectionManager GetKerringConnectionManager()
        {
            return new ConnectionManager(GetKerringDevConnectionInfo());
        }

        private static ConnectionManager GetTestCIConnectionManager()
        {
            return new ConnectionManager(GetTestCIConnectionInfo());
        }

        private static ConnectionInfo GetKerringDevConnectionInfo()
        {
            return new ConnectionInfo
            {
                ServiceAddress = "https://keringdev.visualstudio.com/",
                UserName = "coola",
                Password = "CoolaHaslo123"
            };
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

        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        private static CodeManager GetKerringDevCodeManager()
        {
            return new CodeManager(GetKerringConnectionManager().GetTfsTeamProjectCollection(),
                "$/ShARe-Evolution/ShARe-TOM", GetLocalWorkingDirectoryPath());
        }

        private static CodeManager GetCITestCodeManager()
        {
            return new CodeManager(GetTestCIConnectionManager().GetTfsTeamProjectCollection(), "$/CITestProject",
                GetLocalWorkingDirectoryPath());
        }

        private static string GetLocalWorkingDirectoryPath()
        {
            return @"C:\Data\Source\";
        }

        private static string GetNewestDirectory()
        {
            var directory = new DirectoryInfo(GetLocalWorkingDirectoryPath());
            return (from f in directory.GetFiles()
                orderby f.LastWriteTime descending
                select f.Name).First();
        }

        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        [Fact]
        public void CheckGettingSolutionFile()
        {
            var solutionFile = CodeManager.GetSolutionFile(GetTestFolderPath);
            Assert.False(string.IsNullOrEmpty(solutionFile));
        }

        private static string GetTestFolderPath => @"C:\Data\Source\ShareTomBuildDir_2016_10_17-11_54_56_ver_9";

        [Fact]
        public void CheckBuild()
        {
            var buildManager = new BuildManager(GetTestFolderPath, "Debug", "Any CPU");
            var buildResult = buildManager.BuildSolution();
            Assert.True(buildResult);
        }

        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testCiConnectionManager = GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = GetCITestCodeManager().GetCode(TestCommits.BuildWrongTestOK);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.False(buildManager.BuildSolution());
        }

        [Fact]
        public void CheckIfGoodCommitIsOK()
        {
            var testCiConnectionManager = GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = GetCITestCodeManager().GetCode(TestCommits.BuildOKTestWrong);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.True(buildManager.BuildSolution());
        }

        [Fact]
        public void SendBrokenBuildMail()
        {
            var tfsTeamProjectCollection = GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            var ciTestCodeManager = GetCITestCodeManager();
            var changsetAuthor = ciTestCodeManager.GetChangsetAuthor(TestCommits.BuildWrongTestOK);
            new MailManager(TestCommits.BuildWrongTestOK, changsetAuthor).SendNoBuildMessageUsingVisualStudioServices(tfsTeamProjectCollection);
        }

        [Fact]
        public void SendBrokenBuildMailConcreteUser()
        {
            var tfsTeamProjectCollection = GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            new MailManager(TestCommits.BuildWrongTestOK, "Michał Kuliński").SendNoBuildMessageUsingVisualStudioServices(tfsTeamProjectCollection);
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
    }
}