using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class BuildSolutionOnCodeTests
    {
        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = new GetTFSCodeTests().GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}