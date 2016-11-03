using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests4
    {
        [Fact]
        public void CheckIfGoodCommitIsOK()
        {
            var testCiConnectionManager = GetTFSCodeTests.GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = GetTFSCodeTests.GetCITestCodeManager().GetCode(GetTFSCodeTests.TestCommits.BuildOKTestWrong);
            var buildManager = new BuildManager(pathToCodeDir, "Debug", "Any CPU");
            Assert.True(buildManager.BuildSolution());
        }
    }
}