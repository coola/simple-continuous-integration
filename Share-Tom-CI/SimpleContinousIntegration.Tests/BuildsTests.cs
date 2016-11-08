using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class BuildsTests
    {
        [Fact]
        public void CheckBuild()
        {
            var buildResult = new GetTFSCodeTests().RetrieveCodeAndBuild(null);
            Assert.True(buildResult);
        }
    }
}