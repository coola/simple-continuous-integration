using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CIInterfaceTests
    {
        [Fact]
        public void CheckCIInterface()
        {
            var getTfsCodeTests = new GetTFSCodeTests();
            var ci = getTfsCodeTests.CiFactory();
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsOnce();
        }
    }
}
