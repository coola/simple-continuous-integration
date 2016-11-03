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

        public static ConnectionManager GetTestCIConnectionManager()
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

        private static CodeManager GetKerringDevCodeManager()
        {
            return new CodeManager(GetKerringConnectionManager().GetTfsTeamProjectCollection(),
                "$/ShARe-Evolution/ShARe-TOM", GetLocalWorkingDirectoryPath());
        }

        public static CodeManager GetCITestCodeManager()
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