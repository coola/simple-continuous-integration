using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests3
    {
        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testCiConnectionManager = GetTFSCodeTests.GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = GetTFSCodeTests.GetCITestCodeManager().GetCode(GetTFSCodeTests.TestCommits.BuildWrongTestOK);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.False(buildManager.BuildSolution());
        }
    }
}