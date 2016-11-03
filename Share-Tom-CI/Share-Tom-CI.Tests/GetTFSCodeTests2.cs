using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests2
    {
        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = GetTFSCodeTests.GetCITestCodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}