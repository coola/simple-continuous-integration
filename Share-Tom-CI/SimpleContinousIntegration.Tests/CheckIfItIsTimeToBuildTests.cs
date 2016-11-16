using SimpleContinousIntegration.Time;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfItIsTimeToBuildTests
    {
        [Fact]
        public void CheckIfItIsTimeToBuild()
        {
            var testUtilities = new TestUtilities();
            var ci = testUtilities.CiFactoryNewest();
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();
            var buildFolderManager= testUtilities.BuildFolderManagerFactory();
            var timeManager = new TimeManager(buildFolderManager);
            Assert.False(timeManager.ItIsTimeToBuild());
        }
    }
}
