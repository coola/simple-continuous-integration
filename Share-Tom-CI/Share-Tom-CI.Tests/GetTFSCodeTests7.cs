using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests7
    {
        [Fact]
        public void CheckBuild()
        {
            var codeFolderPath = GetTFSCodeTests.GetCode();
            var buildManager = new BuildManager(codeFolderPath, "Debug", "Any CPU");
            var buildResult = buildManager.BuildSolution();
            Assert.True(buildResult);
        }
    }
}