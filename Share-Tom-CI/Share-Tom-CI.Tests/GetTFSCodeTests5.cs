using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests5
    {
        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeFolderPath = GetTFSCodeTests.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }
    }
}