using SimpleContinousIntegration.BuildStrategies;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class VariuosCommitTests
    {
        [Fact]
        public void CheckIfFailedCommitFails()
        {
            var testUtilities = new TestUtilities();
            var testCiConnectionManager = testUtilities.GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var pathToCodeDir = testUtilities.GetCITestCodeManager().GetCode(TestUtilities.TestCommits.BuildWrongTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, TestUtilities.testDebugConfiguration, TestUtilities.testAnyCPUPlatform);
            Assert.False(buildManager.BuildSolution());
        }
    }
}