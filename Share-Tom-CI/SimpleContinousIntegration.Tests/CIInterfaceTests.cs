using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CIInterfaceTests
    {
        [Fact]
        public void CheckCIInterface()
        {
            var getTfsCodeTests = new TestUtilities();
            var ci = getTfsCodeTests.CiFactory();
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();
        }
    }
}
