using System.Linq;
using SimpleContinousIntegration.BuildStrategies;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class TestAssemblyFinderMsBuildManagerTests
    {
         [Fact]
        public void TestAssemblyFinderMsBuildManager()
        {
            var pathToCodeDir = new GetTFSCodeTests().GetCITestCodeManager().GetCode();
            var buildManager = new MsBuildBuildManager(pathToCodeDir, GetTFSCodeTests.testDebugConfiguration, GetTFSCodeTests.testAnyCPUPlatform);
            buildManager.BuildSolution();
            var buildManagerCurrentAssemblyList = buildManager.CurrentAssemblyList;
            Assert.NotEmpty(buildManagerCurrentAssemblyList);
            Assert.True(buildManagerCurrentAssemblyList.All(a => a.EndsWith(".dll")));
        }
    }
}
