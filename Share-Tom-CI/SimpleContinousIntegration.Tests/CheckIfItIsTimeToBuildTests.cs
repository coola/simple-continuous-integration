using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfItIsTimeToBuildTests
    {
        [Fact]
        public void CheckIfItIsTimeToBuild()
        {
            var ci = new TestUtilities().CiFactoryNewest();
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();
            Assert.False(ci.ItIsTimeToBuild());
        }
    }
}
