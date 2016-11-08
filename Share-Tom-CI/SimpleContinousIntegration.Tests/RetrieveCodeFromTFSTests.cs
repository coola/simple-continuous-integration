using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class RetrieveCodeFromTFSTests
    {
        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeFolderPath = new GetTFSCodeTests().GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}
