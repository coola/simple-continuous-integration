using SimpleContinousIntegration.BuildStrategies;
using SimpleContinousIntegration.Test;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class RunTestsSuccessfulyTests
    {
         [Fact]
        public void RunTestsSuccessfuly()
        {
            var getTfsCodeTests = new TestUtilities();
            var pathToCodeDir = getTfsCodeTests.GetCITestCodeManager().GetCode(TestUtilities.TestCommits.BuildOKTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, TestUtilities.testDebugConfiguration, TestUtilities.testAnyCPUPlatform);
            buildManager.BuildSolution();
            var testManager = new TestManager(getTfsCodeTests.testWorkingDirectoryPath, buildManager.CurrentAssemblyList);
            var testsResult = testManager.RunTests();
            Assert.True(testsResult);
        }
    }
}
