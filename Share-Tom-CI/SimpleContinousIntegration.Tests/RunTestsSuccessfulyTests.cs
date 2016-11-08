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
            var getTfsCodeTests = new GetTFSCodeTests();
            var pathToCodeDir = getTfsCodeTests.GetCITestCodeManager().GetCode(GetTFSCodeTests.TestCommits.BuildOKTestOK);
            var buildManager = new MsBuildBuildManager(pathToCodeDir, GetTFSCodeTests.testDebugConfiguration, GetTFSCodeTests.testAnyCPUPlatform);
            buildManager.BuildSolution();
            Assert.True(new TestManager(getTfsCodeTests.testWorkingDirectoryPath, buildManager.CurrentAssemblyList).RunTests());
        }
    }
}
