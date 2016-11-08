using System;
using System.IO;
using Microsoft.TeamFoundation.Common;
using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Results;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class GetTFSCodeTests
    {
        public GetTFSCodeTests()
        {
            Directory.CreateDirectory(new TestUtilities().testWorkingDirectoryPath);
        }

        [Fact]
        public void CheckIfWeHaveWorkingFolder()
        {
            var localWorkingDirectoryPath = new TestUtilities().testWorkingDirectoryPath;
            Directory.CreateDirectory(localWorkingDirectoryPath);
            Directory.Exists(localWorkingDirectoryPath);
        }

        [Fact]
        public void CheckCodeManagerForEmptyFolder()
        {
            Assert.Throws<ArgumentException>(() => new TestUtilities().CreateTestProjectCodeManager(string.Empty));
        }


        [Fact]
        public void CheckIfWeHaveConnectionToTFS()
        {
            var connectionManager = new TestUtilities().GetTestCIConnectionManager();
            var validate = connectionManager.Validate();
            Assert.True(validate);
        }


        [Fact]
        public void SendBrokenBuildMail()
        {
            var testUtilities = new TestUtilities();
            var tfsTeamProjectCollection = testUtilities.GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            var ciTestCodeManager = testUtilities.GetCITestCodeManager();
            var changsetAuthor = ciTestCodeManager.GetChangsetAuthor(TestUtilities.TestCommits.BuildWrongTestOK);
            new MailManager(TestUtilities.TestCommits.BuildWrongTestOK, changsetAuthor)
                .SendNoBuildMessageUsingVisualStudioServices(
                    tfsTeamProjectCollection);
        }

        [Fact]
        public void SendBrokenBuildMailConcreteUser()
        {
            var tfsTeamProjectCollection =
                new TestUtilities().GetTestCIConnectionManager().GetTfsTeamProjectCollection();
            new MailManager(TestUtilities.TestCommits.BuildWrongTestOK, "Michał Kuliński")
                .SendNoBuildMessageUsingVisualStudioServices
                (tfsTeamProjectCollection);
        }

        [Fact]
        public void CheckOwner()
        {
            var ciTestCodeManager = new TestUtilities().GetCITestCodeManager();
            var changsetAuthor = ciTestCodeManager.GetChangsetAuthor(TestUtilities.TestCommits.BuildWrongTestOK);
            Assert.Equal("Coola", changsetAuthor);
        }


        [Fact]
        public void CheckDirectorySizeCounter()
        {
            var directorySize = new TestUtilities().DirectorySize();
            Assert.NotEqual(0, directorySize);
        }

        [Fact]
        public void CheckDirectorySizeCounterTrimFunctionality()
        {
            var maintananceManager = new TestUtilities().GetMaintananceManager();
            maintananceManager.TrimBuildDirectoryToMaxSize();
        }

        [Fact]
        public void CheckGettingOFCurrentMsBuildPath()
        {
            var currentMsBuildPath = MsBuildBuildManager.GetCurrentMsBuildPath();
            Assert.False(currentMsBuildPath.IsNullOrEmpty());
        }
    }
}