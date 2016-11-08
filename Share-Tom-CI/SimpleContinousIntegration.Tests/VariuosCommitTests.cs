using SimpleContinousIntegration.BuildStrategies;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class VariuosCommitTests
    {
        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testCiConnectionManager = new GetTFSCodeTests().GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = new GetTFSCodeTests().GetCITestCodeManager().GetCode(GetTFSCodeTests.TestCommits.BuildWrongTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, GetTFSCodeTests.testDebugConfiguration, GetTFSCodeTests.testAnyCPUPlatform);
            Assert.False(buildManager.BuildSolution());
        }
    }
}