using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class RetrieveCodeFromTFSTests
    {
        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeFolderPath = new TestUtilities().GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}
